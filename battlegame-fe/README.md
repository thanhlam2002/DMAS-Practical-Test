# Battle Game – Azure Functions + React

## 1. Mục tiêu
- DB MySQL gồm 3 bảng: `player`, `asset`, `player_asset`.
- API Azure Functions:
  - POST /api/registerplayer
  - POST /api/createasset
  - POST /api/assignasset (bonus)
  - GET  /api/getassetsbyplayer
- Website React hiển thị bảng report.

## 2. Yêu cầu môi trường
- Node 18+ (hoặc 20+), npm
- .NET 8 SDK, Azure Functions Core Tools v4
- MySQL (XAMPP) local hoặc Azure Database for MySQL

## 3. Tạo DB & seed
```sql
-- file: sql/seed.sql
CREATE DATABASE IF NOT EXISTS battlegame;
USE battlegame;

CREATE TABLE IF NOT EXISTS player (
  player_id INT AUTO_INCREMENT PRIMARY KEY,
  player_name VARCHAR(100), full_name VARCHAR(100),
  age INT, level INT
);

CREATE TABLE IF NOT EXISTS asset (
  asset_id INT AUTO_INCREMENT PRIMARY KEY,
  asset_name VARCHAR(100), description VARCHAR(255), type VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS player_asset (
  id INT AUTO_INCREMENT PRIMARY KEY,
  player_id INT, asset_id INT,
  FOREIGN KEY (player_id) REFERENCES player(player_id),
  FOREIGN KEY (asset_id) REFERENCES asset(asset_id)
);

INSERT INTO player (player_name, full_name, age, level)
VALUES ('Player 1','Nguyen Van A',20,10), ('Player 2','Tran Van B',19,3), ('Player 3','Le Thi C',23,10);

INSERT INTO asset (asset_name, description, type)
VALUES ('Hero 1','Strong hero','Hero'), ('Hero 2','Quick hero','Hero');

INSERT INTO player_asset (player_id, asset_id) VALUES (1,1),(2,2),(3,1);
