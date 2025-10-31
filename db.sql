CREATE DATABASE battlegame CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE battlegame;

CREATE TABLE player (
    player_id INT AUTO_INCREMENT PRIMARY KEY,
    player_name VARCHAR(100) NOT NULL,
    full_name VARCHAR(100),
    age INT,
    level INT DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE asset (
    asset_id INT AUTO_INCREMENT PRIMARY KEY,
    asset_name VARCHAR(100) NOT NULL,
    description VARCHAR(255),
    type VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE player_asset (
    id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    asset_id INT NOT NULL,
    FOREIGN KEY (player_id) REFERENCES player(player_id),
    FOREIGN KEY (asset_id) REFERENCES asset(asset_id)
);
