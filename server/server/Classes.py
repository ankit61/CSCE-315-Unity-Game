import threading
import asyncio
import websockets
import sys
import json

#unused as of now
#TODO! Do something with me
DEFAULT_ROOM_SIZE = 4

class Room():

    meta, player = range(2)
    
    def __init__(self, path, playerSlots, defaultMetaData={}):
        self.path = path
        self.metaData = defaultMetaData
        self.playerData = {}
        self.maxPlayers = playerSlots

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
                self.d[key[1]] = value
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

    @staticmethod
    async def notify_users(iterable, message):
        for websocket in iterable:
            await websocket.send(json.dumps(message))

    @staticmethod
    async def register(websocket, path):
        #temporary ID for the purposes of testing
        userID = websocket.__hash__()

        if path not in Server.rooms:
            print("room {0} created".format(path))
            Server.rooms[Room.meta, path] = Room(
                path, 
                DEFAULT_ROOM_SIZE,
                {
                    "count" : 0,
                    "userCount" : 0
                }
            )
        
        if websocket not in Server.rooms[Room.player, path]:
            print("added user {0} to room {1}".format(websocket, path))
            Server.rooms[Room.player, path, websocket] = {}
        
        Server.rooms[Room.meta, path]["userCount"] += 1

        #something like the user ID will be the value of this in the future, for right now it is unused
        message = {
            "newuser" : userID
        }

        await Server.notify_users(
            Server.rooms[Room.player, path],
            message
        )

    @staticmethod
    async def unregister(websocket, path):  
        #temporary ID for the purposes of testing
        userID = websocket.__hash__()

        print("removed user {0} to room {1}".format(websocket, path))
        del Server.rooms[Room.player, path, websocket]
        
        if Server.rooms[Room.meta, path]["userCount"] == 1:
            print("room {0} removed".format(path))
            del Server.rooms[Room.meta, path] 
        else:
            #something like the user ID will be the value of this in the future, for right now it is unused
            message = {
                "deaduser" : userID
            }

            await Server.notify_users(
                Server.rooms[Room.player, path],
                message
            )    

    @staticmethod
    async def increment(websocket, path):
        Server.rooms[Room.meta, path]["count"] += 1
        return {"count": Server.rooms[Room.meta, path]["count"]}

    @staticmethod
    async def decrement(websocket, path):
        Server.rooms[Room.meta, path]["count"] -= 1
        return {"count": Server.rooms[Room.meta, path]["count"]}

    @staticmethod
    async def ping(websocket, path):
        return {"message" : "Ping Success: " + str(websocket.__hash__())}

    @staticmethod
    async def action(websocket, path):
        return {"actionBy": str(websocket.__hash__())}


    @staticmethod 
    async def parse_JSON(websocket, path, message):
        for action in message['action']:
            response = await Server.actionmap[action](websocket,path)
            if response: 
                await Server.notify_users(Server.rooms[Room.player, path], response)
        

    @staticmethod
    async def dispatcher(websocket, path):
        #print(websocket)
        #print(path)

        path = path[1:]
        await Server.register(websocket, path)
        try:
            #This behaves like a wait function more than a for loop
            async for message in websocket:
                # we will also include a path parsing system here
                # however this is not necessary right now
                await Server.parse_JSON(websocket, path, json.loads(message))
        finally:    
            await Server.unregister(websocket, path)

    @staticmethod
    def main():
        addr = "0.0.0.0"
        port = 8080

        start_server = websockets.serve(
            Server.dispatcher, 
            addr, 
            port
        )
        
        print("Server started on {addr}:{port}".format(addr=addr, port=port))
        asyncio.get_event_loop().run_until_complete(start_server)
        asyncio.get_event_loop().run_forever()
        return

    rooms = UserIndex() 
    actionmap = {
        "increment": lambda websocket, path: Server.increment(websocket, path),
        "decrement": lambda websocket, path: Server.decrement(websocket, path),
        "ping" : lambda websocket, path: Server.ping(websocket, path),
        "action" : lambda websocket, path: Server.action(websocket, path)
    }


    

