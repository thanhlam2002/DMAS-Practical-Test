-- Active: 1727270994892@@127.0.0.1@3306@battlegame
INSERT INTO player (player_name, full_name, age, level) VALUES
('Player 1', 'Nguyen Van A', 20, 10),
('Player 2', 'Tran Van B', 19, 3),
('Player 3', 'Le Thi C', 23, 10);

INSERT INTO asset (asset_name, description, type) VALUES
('Hero 1', 'Strong Hero', 'Hero'),
('Hero 2', 'Quick Hero', 'Hero');

INSERT INTO player_asset (player_id, asset_id) VALUES
(1, 2),
(2, 2),
(3, 3);


SELECT p.player_name, p.level, p.age, a.asset_name
FROM player p
JOIN player_asset pa ON p.player_id = pa.player_id
JOIN asset a ON pa.asset_id = a.asset_id;