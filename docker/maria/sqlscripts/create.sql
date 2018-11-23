CREATE DATABASE IF NOT EXISTS `rebounddb`;

CREATE TABLE IF NOT EXISTS `rebounddb`.`users` (
    username 
        VARCHAR(255) 
        NOT NULL
        PRIMARY KEY,
    score
        int
        NOT NULL
)ENGINE=Aria;

CREATE TABLE IF NOT EXISTS `rebounddb`.`currentplayers` (
    username
        VARCHAR(255)
        NOT NULL
        PRIMARY KEY,
    room
        VARCHAR(8)
        NOT NULL,
    socket_hash
        BIGINT 
        SIGNED
        not null,
    CONSTRAINT `fk_username`
		FOREIGN KEY (username) REFERENCES users (username)
		ON DELETE CASCADE
		ON UPDATE CASCADE
)ENGINE=MEMORY;