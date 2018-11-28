import pymysql

class conn():
    def __init__(self, host, port, user, password):
        self.host=host
        self.port=port
        self.user=user
        self.password=password
        self.charset='utf8mb4'
        self.connection = None
    def __enter__(self):
        self.connection = pymysql.connect(host=self.host, port=self.port, user=self.user, password=self.password)
        return self.connection.cursor()
    def __exit__(self, err1, err2, err3):
        self.connection.close()
        self.connection = None



upsertuser_sql = [
    """PREPARE `upsert_stmt` FROM
        'INSERT 
        INTO `rebounddb`.`users` (username, score)
        VALUES (?,?)
        ON DUPLICATE KEY UPDATE
        username=?;';
    """,
    "SET @name := \"{name}\";",
    "SET @default_score := 0;",
    "EXECUTE `upsert_stmt` USING @name, @default_score, @name;"
]

incscore_sql = [
    """PREPARE `inc_stmt` FROM
        'UPDATE `rebounddb`.`users`
        SET score = score + 1
        WHERE username=?
        LIMIT 1;';
    """,
    "SET @name := \"{name}\";",
    "EXECUTE `inc_stmt` USING @name;"
]

getscore_sql = [
    """PREPARE `get_stmt` FROM
        'SELECT (score) 
        FROM `rebounddb`.`users`
        WHERE username=?
        LIMIT 1;';
    """,
    "SET @name := \"{name}\";",
    "EXECUTE `get_stmt` USING @name;"
]

loginuser_sql = [
    """PREPARE `login_stmt` FROM
        'INSERT 
        INTO `rebounddb`.`currentplayers` (username, room, socket_hash)
        VALUES (?,?,?)
        ON DUPLICATE KEY UPDATE
        room=?, socket_hash=?;';
    """,
    "SET @name := \"{name}\";",
    "SET @room := \"{room}\";",
    "SET @hash := \"{hash}\";",
    "EXECUTE `login_stmt` USING @name, @room, @hash, @room, @hash;"
]

logoutuser_sql = [
    """PREPARE `logout_stmt` FROM
        'DELETE FROM `rebounddb`.`currentplayers`
        WHERE username=?
        LIMIT 1;';
    """,
    "SET @name := \"{name}\"",
    "EXECUTE `logout_stmt` USING @name;"
]

statususer_sql = [
    """PREPARE `status_stmt` FROM
        'SELECT * 
        FROM `rebounddb`.`currentplayers`
        WHERE username=?
        LIMIT 1;';
    """,
    "SET @name := \"{name}\";",
    "EXECUTE `status_stmt` USING @name;"
]

leaderboard_sql = [
    """PREPARE `top_stmt` FROM
            'SELECT *
            FROM `rebounddb`.`users`
            ORDER BY score DESC
            LIMIT 20;';
    """,
    "EXECUTE `top_stmt`;"
]


def sendquery(cursor, subs, query):
    return list(map(lambda val: cursor.execute( val.format(**subs)), query))


def upsertuser(name):
    with conn("maria", 3306, "bot", "csce315kerne") as cursor:
        sendquery(cursor, {"name" : name}, upsertuser_sql)
    return

def incrementscore(name):
    with conn("maria", 3306, "bot", "csce315kerne") as cursor:
        sendquery(cursor, {"name" : name}, incscore_sql)
    return

def getscore(name):
    with conn("maria", 3306, "bot", "csce315kerne") as cursor:
        sendquery(cursor, {"name" : name}, getscore_sql)
        score = cursor.fetchone()
    return score[0]

def loginuser(name, room, websocket_hash):
    with conn("maria", 3306, "bot", "csce315kerne") as cursor:
        sendquery(cursor, {"name":name,"room":room,"hash":websocket_hash}, loginuser_sql)
    return 

def logoutuser(name):
    with conn("maria", 3306, "bot", "csce315kerne") as cursor:
        sendquery(cursor, {"name":name}, logoutuser_sql)
    return 

def statususer(name):
    with conn("maria", 3306, "bot", "csce315kerne") as cursor:
        sendquery(cursor, {"name":name}, statususer_sql)
        score = cursor.fetchone()
    return score

def getleader():
    with conn("maria", 3306, "bot", "csce315kerne") as cursor:
        sendquery(cursor, {}, leaderboard_sql)
        score = cursor.fetchall()
    return score
