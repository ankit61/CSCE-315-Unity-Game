#!/usr/bin/env python3

import threading
import wsapp
import httpapp

http_servers = [
    threading.Thread(target=httpapp.start, args=("0.0.0.0", 8081, httpapp.getroom_HTTPHandler,)),
    threading.Thread(target=httpapp.start, args=("0.0.0.0", 8082, httpapp.makeuser_HTTPHandler,)),
    threading.Thread(target=httpapp.start, args=("0.0.0.0", 8083, httpapp.getscore_HTTPHandler,)),
    threading.Thread(target=httpapp.start, args=("0.0.0.0", 8084, httpapp.incscore_HTTPHandler,)),
    threading.Thread(target=httpapp.start, args=("0.0.0.0", 8085, httpapp.checkroom_HTTPHandler,)),
]

if __name__ == "__main__":
    for server in http_servers:
        server.start()
    wsapp.start()
    for server in http_servers:
        server.join()
