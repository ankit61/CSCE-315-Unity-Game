# Starting the server

Note that this is a demonstration server to show websocket's performance. Think of this server as a tabula rosa with an example of its functionality

## With python
The server requires the the user have python 3.6 or greater installed on their machine for it to run. If this is satisfied, either through a virtual env or it already existing on the system (most distros *DO NOT* have this supported by default) simply start the server with these commands from this directory.
```
$ pip3 install --no-cache-dir -r ./docker/websocket-server/requirements.txt
$ [path/to/python3.6] server
```

You will also need to open the `index.html` file on your own in the browser (server does not support, and will not, support static file transfer)

## With docker
Install docker (this is distro dependant). Navigate to the `./docker` folder and run this command, assuming you have the proper permissions.

```
$ docker-compose build
$ docker-compose up
```

If you want it to run in the background (docker does not run in the background normally, it needs a flag)

```
$ docker-compose build
$ docker-compose up -d 
```

To kill the background docker processes

```
$ docker stop $(docker ps -aq)
```

Note that this docker config also supports nginx, so simply navigate to localhost:8080 to get the full experience (no need to load the file yourself)