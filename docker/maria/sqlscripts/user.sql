CREATE USER IF NOT EXISTS `bot`@'%' IDENTIFIED BY 'csce315kerne';
USE `rebounddb`;
GRANT SELECT ON `users` TO `bot`@'%';
GRANT INSERT ON `users` TO `bot`@'%';
GRANT UPDATE ON `users` TO `bot`@'%';