from roomio.structures import Server
from collections import deque
from sqlfunctions import loginuser, logoutuser
import functools
import json

DEFAULT_SIZE = 4

#data that is shared between all users
@Server.shareddata
class Shared():
    def __init__(self):
        self.count = 0
        self.queueCount = 0
        self.queue = deque(range(DEFAULT_SIZE))
        self.players = set()
        self.states = [None for _ in range(DEFAULT_SIZE)]

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
        self.username = None

@Server.register(Server.TOSELF)
def register_user_self(websocket, path):

    userNum = Server.fetch_room_data(path).addUser()
    Server.fetch_player_data(websocket, path).slot = userNum
    player_list = Server.fetch_player_list(path)
    players = [[player_list[player].slot, player_list[player].username] for player in player_list]

    return {
        "method": "joininfo", 
        "slot": userNum, 
        "players": players
    }

@Server.message("join", Server.TOOTHERS)
def register_user_others(websocket, path, message):
    Server.fetch_player_data(websocket, path).username = message["data"]["username"]
    print("Joined")
    
    loginuser(message["data"]["username"], path, websocket.__hash__())

    return {
        "method": "newuser", 
        "slot": Server.fetch_player_data(websocket, path).slot,
        "username": message["data"]["username"]
    }

@Server.unregister(Server.TOOTHERS)
def unregister_user(websocket, path):

    logoutuser(Server.fetch_player_data(websocket, path).username)

    num, name = Server.fetch_player_data(websocket, path).slot, Server.fetch_player_data(websocket, path).username
    Server.fetch_room_data(path).states[num] = None
    Server.fetch_room_data(path).removeUser(num)
    
    return {
        "method": "deaduser", 
        "deaduser": [num, name]
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

@Server.message("pos_update", Server.TOSELF)
def pos_update(websocket, path, message):
    data = message["data"]
    slot = Server.fetch_player_data(websocket, path).slot
    Server.fetch_room_data(path).states[slot] = data

    return Server.fetch_room_data(path).states




def start():
    Server.start("0.0.0.0", 8080)