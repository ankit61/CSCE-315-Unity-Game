#!/usr/bin/env python3

import threading
import wsapp
import httpapp

if __name__ == "__main__":
    http_server = threading.Thread(target=httpapp.start)
    http_server.start()
    wsapp.start()
    http_server.join()
