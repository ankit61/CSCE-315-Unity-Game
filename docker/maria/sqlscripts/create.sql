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
