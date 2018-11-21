CREATE USER IF NOT EXISTS `bot`@'%' IDENTIFIED BY 'csce315kerne';
GRANT SELECT ON `rebounddb`.* TO `bot`@'%';
GRANT INSERT ON `rebounddb`.* TO `bot`@'%';
GRANT UPDATE ON `rebounddb`.* TO `bot`@'%';
GRANT DELETE ON `rebounddb`.* TO `bot`@'%';