#!/usr/bin/env python

# WS server that sends messages at random intervals

from queue import Queue
from Classes import Server
from endpoints import start

if __name__ == "__main__":
    start()