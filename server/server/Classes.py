import threading
import asyncio
import websockets
import sys
import json 
import functools

#unused as of now
#TODO! Do something with me
DEFAULT_ROOM_SIZE = 4


class Room():
    meta, player = range(2)
    def __init__(self, path, defaultMetaData={}):
        self.path = path
        self.metaData = defaultMetaData
        self.playerData = {}

class UserIndex():

    def __init__(self):
        self.d = {}
        self.lock = threading.Lock() 
    
    def __contains__(self, key):
        with self.lock:
            if key in self.d:
                return True
            else:
                return False

    def __getitem__(self, key):
        with self.lock:
            if key[0] == Room.meta:
                return self.d[key[1]].metaData
            elif len(key) == 2:
                return self.d[key[1]].playerData
            else:
                return self.d[key[1]].playerData[key[2]]
                
    def __setitem__(self, key, value):
        with self.lock:
            if key[0] == Room.meta:
                if key[1] not in self.d:
                    self.d[key[1]] = Room(key[1], value)
                else:
                    self.d[key[1]].metaData = value 
            elif len(key) == 2:
                self.d[key[1]].playerData = value
            else:
                self.d[key[1]].playerData[key[2]] = value
    
    def __delitem__(self, key):
        with self.lock:
            if key[0] == Room.meta:
                del self.d[key[1]]
            elif len(key) == 2:
                del self.d[key[1]].playerData
            else:
                del self.d[key[1]].playerData[key[2]]

class Server():  

    TOALL, TOSELF, TOOTHERS = range(3)
    _handlers = {}
    _rooms = UserIndex()
    _PersonalClass = object
    _SharedClass = object
    _open = []
    _close = []
    
    @staticmethod
    async def dispatcher(websocket, path):
        path = path[1:]
        
        check = await Server.default_register(websocket, path)
        if check:
            try:
                #This behaves like a wait function more than a for loop
                async for message in websocket:
                    # we will also include a path parsing system here
                    # however this is not necessary right now
                    json_message = json.loads(message)
                    for purpose in json_message['method']:
                        await Server.route(purpose, websocket, path, json_message)           
            finally:    
                await Server.default_unregister(websocket, path)

    @staticmethod
    def start(host, port):
        addr = str(host)
        port = int(port)

        start_server = websockets.serve(
            Server.dispatcher, 
            addr, 
            port
        )
        
        print("Server started on {addr}:{port}".format(addr=addr, port=port))
        asyncio.get_event_loop().run_until_complete(start_server)
        asyncio.get_event_loop().run_forever()
        return

    #uses the request path to distinguish rooms, keeps private and public data about each users
    #assumes all responses are in JSON 

    #####################default functions##########################
    @staticmethod
    async def default_register(websocket, path):
        
        #default actions
        if path not in Server._rooms:
            print(f"Room {path} created")
            Server._rooms[Room.meta, path] = Server._SharedClass() 
        if Server._rooms[Room.meta, path].userCount < DEFAULT_ROOM_SIZE:  
            Server._rooms[Room.player, path, websocket] = Server._PersonalClass()
            Server._rooms[Room.meta, path].userCount += 1
        
            print(f"player created {websocket.__hash__()}")
            #custom action
            for func in Server._open:
                await func(websocket, path)
            return True
        else:
            return False
      
    @staticmethod
    async def default_unregister(websocket, path):
        
        #custom actions
        print(f"player killed {websocket.__hash__()}")
        for func in Server._close:
            await func(websocket, path)

        #default actions
        Server._rooms[Room.meta, path].userCount -= 1
        del Server._rooms[Room.player, path, websocket]
        if Server._rooms[Room.meta, path].userCount == 0:
            print(f"Room {path} destroyed")
            del Server._rooms[Room.meta, path]
        
        return True

    @staticmethod
    def fetch_player_data(websocket, path): 
        return Server._rooms[Room.player, path, websocket]
    
    @staticmethod
    def fetch_player_list(path):
        return Server._rooms[Room.player, path]
    
    @staticmethod
    def fetch_room_data(path):
        return Server._rooms[Room.meta, path]
    
    @staticmethod
    async def route(identifier, websocket, path, message):
        for func in Server._handlers[identifier]:
            await func(websocket, path, message)
    
    @staticmethod
    async def responseToAll(websocket, path, message):
        ready = json.dumps(message)
        for ws in Server._rooms[Room.player, path]:
            await ws.send(ready)
    
    @staticmethod
    async def responseToSelf(websocket, path, message):
        ready = json.dumps(message)
        await websocket.send(ready)

    @staticmethod
    async def responseToOthers(websocket, path, message):
        ready = json.dumps(message)
        for ws in Server._rooms[Room.player, path]:
            if ws != websocket:
                await ws.send(ready)

    ########################decorators##############################

    @staticmethod
    def shareddata(original):
        oldinit = original.__init__

        def __init__(self):
            oldinit(self)
            self.userCount = 0

        original.__init__ = __init__ 

        Server._SharedClass = original
        return original

    @staticmethod
    def personaldata(original):
        Server._PersonalClass = original
        return original

    @staticmethod
    def message(identifier, method):
        def message_decorator(func):
            @functools.wraps(func)
            async def message_wrapper(*args, **kwargs):
                message = func(*args, **kwargs)
                if method == Server.TOALL:
                    await Server.responseToAll(args[0], args[1], message)
                elif method == Server.TOSELF:
                    await Server.responseToSelf(args[0], args[1], message)
                elif method == Server.TOOTHERS:
                    await Server.responseToOthers(args[0], args[1], message)
            if identifier in Server._handlers:
                Server._handlers[identifier].append(message_wrapper)
            else:
                Server._handlers[identifier] = [message_wrapper]
            return message_wrapper
        return message_decorator    

    @staticmethod
    def register(method):
        def message_decorator(func):
            @functools.wraps(func)
            async def message_wrapper(*args, **kwargs):
                message = func(*args, **kwargs)
                if method == Server.TOALL:
                    await Server.responseToAll(args[0], args[1], message)
                elif method == Server.TOSELF:
                    await Server.responseToSelf(args[0], args[1], message)
                elif method == Server.TOOTHERS:
                    await Server.responseToOthers(args[0], args[1], message)
            Server._open.append(message_wrapper)
            return message_wrapper
        return message_decorator   

    @staticmethod
    def unregister(method):
        def message_decorator(func):
            @functools.wraps(func)
            async def message_wrapper(*args, **kwargs):
                message = func(*args, **kwargs)
                if method == Server.TOALL:
                    await Server.responseToAll(args[0], args[1], message)
                elif method == Server.TOSELF:
                    await Server.responseToSelf(args[0], args[1], message)
                elif method == Server.TOOTHERS:
                    await Server.responseToOthers(args[0], args[1], message)
            Server._close.append(message_wrapper)
            return message_wrapper
        return message_decorator     
    
    #######################
    

