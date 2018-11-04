from Classes import UserIndex, Server
from collections import deque
import functools
import json

#data that is shared between all users
@Server.shareddata
class Shared():
    def __init__(self):
        self.count = 0
        self.queueCount = 0
        self.queue = deque(range(4))
        self.players = set()

    def addUser(self):
        num = self.queue.popleft()
        self.players.add(num)
        self.queueCount += 1
        return num

    def removeUser(self, num):
        self.queue.append(num)
        self.players.discard(num)
        self.queueCount -= 1

#data that is reachable by all users but is unique to the player
@Server.personaldata
class Personal():
    def __init__(self):
        self.slot = None

@Server.register(Server.TOSELF)
def register_user_self(websocket, path):

    userNum = Server.fetch_room_data(path).addUser()
    Server.fetch_player_data(websocket, path).slot = userNum

    return {
        "method": "joininfo", 
        "slot": userNum, 
        "players": list(Server.fetch_room_data(path).players)
    }

@Server.register(Server.TOOTHERS)
def register_user_others(websocket, path):

    return {
        "method": "newuser", 
        "slot": Server.fetch_player_data(websocket, path).slot
    }

@Server.unregister(Server.TOOTHERS)
def unregister_user(websocket, path):

    num = Server.fetch_player_data(websocket, path).slot
    Server.fetch_room_data(path).removeUser(num)

    return {
        "method": "deaduser", 
        "deaduser": num
    }

@Server.message("increment", Server.TOALL)
def increment(websocket, path, message):
    Server.fetch_room_data(path).count += 1
    return {
        "method": "increment", 
        "count" : Server.fetch_room_data(path).count
    }

@Server.message("decrement", Server.TOALL)
def decrement(websocket, path, message):
    Server.fetch_room_data(path).count -= 1
    return {
        "method": "decrement", 
        "count" : Server.fetch_room_data(path).count
    }

@Server.message("ping", Server.TOSELF)
def ping(websocket, path, message):
    print(f"Received Ping from {websocket.__hash__()}")
    return {
        "method": "ping", 
        "message" : "Ping Success: " + str(websocket.__hash__())
    }

@Server.message("action", Server.TOOTHERS)
def action(websocket, path, message):     
    data = message['data']
    return {
        "method": "action",
        "slot": Server.fetch_player_data(websocket, path).slot,
        "data" : data
    }

def start():
    Server.start("0.0.0.0", 8080)