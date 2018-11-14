#from roomio.structures import Server
import http.server
import socketserver
import json
import random
from roomio.structures import Server

class getroom_HTTPHandler(http.server.BaseHTTPRequestHandler):
    def generateRoom(self):
        path = "0000000" #we know this is an error if this happens
        while True:
            num = random.randrange(268435456, 4294967295)
            path = str(hex(num))[2:]
            if not Server.checkRoom(path):
                break
        return path 
        


    def do_GET(self):

        path = self.generateRoom()

        self.send_response(200)
 
        # Send headers
        self.send_header('Content-type','application/json')
        self.end_headers()
 
        # Send message back to client
        message = json.dumps({"room": path})
        # Write content as utf-8 data
        self.wfile.write(bytes(message, "utf8"))



port = 8081
host = "0.0.0.0"

def start():
    with socketserver.TCPServer((host, port), getroom_HTTPHandler) as httpd:
        print("HTTP serving {host}:{port}".format(host=host, port=port))
        httpd.serve_forever()