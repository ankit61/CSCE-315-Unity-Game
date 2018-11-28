#from roomio.structures import Server
import http.server
import socketserver
import json
import random
import sys
from roomio.structures import Server
from sqlfunctions import upsertuser, getscore, incrementscore, statususer, getleader

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

class makeuser_HTTPHandler(http.server.BaseHTTPRequestHandler):
    def do_POST(self):
        content_len = int(self.headers.get('content-length', 0))
        try:
            post_json = json.loads(self.rfile.read(content_len))
            upsertuser(post_json["username"])
            self.send_response(200)
        except Exception as e:
            print(e, file=sys.stderr)       
            self.send_response(400)
        finally:
            self.end_headers()

class getscore_HTTPHandler(http.server.BaseHTTPRequestHandler):
    def do_POST(self):
        content_len = int(self.headers.get('content-length', 0))
        message = ""
        try:
            post_json = json.loads(self.rfile.read(content_len))
            score = getscore(post_json["username"])
            message = json.dumps({"score": score})
            self.send_response(200)
            self.send_header('Content-type','application/json')
            self.send_header('Content-length', len(message))
        except Exception as e:
            print(e, file=sys.stderr)       
            self.send_response(400)
        finally:
            self.end_headers()
            self.wfile.write(bytes(message, "utf8"))

class incscore_HTTPHandler(http.server.BaseHTTPRequestHandler):
    def do_POST(self):
        content_len = int(self.headers.get('content-length', 0))
        try:
            post_json = json.loads(self.rfile.read(content_len))
            incrementscore(post_json["username"])
            self.send_response(200)
        except Exception as e:
            print(e, file=sys.stderr)       
            self.send_response(400)
        finally:
            self.end_headers()

class checkroom_HTTPHandler(http.server.BaseHTTPRequestHandler):
    def do_POST(self):
        content_len = int(self.headers.get('content-length', 0))
        message = ''
        try:
            post_json = json.loads(self.rfile.read(content_len))
            check = Server.checkRoom(post_json["room"])
            message = json.dumps({"exists": check})
            self.send_response(200)
            self.send_header('Content-type','application/json')
            self.send_header('Content-length', len(message))
        except Exception as e:
            print(e, file=sys.stderr)       
            self.send_response(400)
        finally:
            self.end_headers()
            self.wfile.write(bytes(message, "utf8"))

class statususer_HTTPHandler(http.server.BaseHTTPRequestHandler):
    def do_POST(self):
        content_len = int(self.headers.get('content-length', 0))
        message = ''
        try:
            post_json = json.loads(self.rfile.read(content_len))
            score = statususer(post_json["username"])
            if score:
                message = {"exists": True, "room": score[1], "socket_hash": score[2]}
            else:
                message = {"exists": False}
            response = json.dumps(message)
            self.send_response(200)
            self.send_header('Content-type','application/json')
            self.send_header('Content-length', len(response))
        except Exception as e:
            print(e, file=sys.stderr)       
            self.send_response(400)
        finally:
            self.end_headers()
            self.wfile.write(bytes(response, "utf8"))     

class leaderboard_HTTPHandler(http.server.BaseHTTPRequestHandler):
    def do_GET(self):
        try:
            top = getleader()
            response = json.dumps({"leaders":top})
            self.send_response(200)
            self.send_header('Content-type','application/json')
            self.send_header('Content-length', len(response))
        except Exception as e:
            print(e, file=sys.stderr)       
            self.send_response(400)
        finally:
            self.end_headers()
            self.wfile.write(bytes(response, "utf8"))


def start(host, port, handler):
    with socketserver.TCPServer((host, port), handler) as httpd:  
        print("HTTP serving {host}:{port}".format(host=host, port=port))
        httpd.serve_forever()