# Battle Game – Azure Functions (.NET 8) + MySQL + React

Website hiển thị bảng “Player – Asset” lấy từ Azure Functions. Dự án dùng **.NET 8 – Azure Functions (isolated worker)**, **MySQL (XAMPP)** và **React**.

---

## 1) Yêu cầu môi trường

- **.NET 8 SDK** + **Azure Functions Core Tools v4**
- **Node.js 18+** (hoặc 20+), **npm**
- **MySQL** (XAMPP hoặc MySQL Community)
- (Tùy chọn) **Azure CLI** nếu deploy cloud

---

## 2) Cấu trúc thư mục

```
.
├─ README.md
├─ sql/
│  └─ seed.sql
├─ backend/                 # Azure Functions (.NET 8 isolated)
│  ├─ BattleGameApi_v1.csproj
│  ├─ Program.cs
│  ├─ host.json
│  ├─ local.settings.json.sample
│  ├─ registerplayer.cs
│  ├─ createasset.cs
│  ├─ assignasset.cs
│  └─ getassetsbyplayer.cs
└─ battlegame-fe/           # React
   ├─ package.json
   ├─ src/App.js
   ├─ src/App.css
   └─ public/index.html
```

---

## 3) CSDL & dữ liệu mẫu

**File:** `sql/seed.sql`

```sql
CREATE DATABASE IF NOT EXISTS battlegame;
USE battlegame;

CREATE TABLE IF NOT EXISTS player (
  player_id   INT AUTO_INCREMENT PRIMARY KEY,
  player_name VARCHAR(100) NOT NULL,
  full_name   VARCHAR(100),
  age         INT,
  level       INT
);

CREATE TABLE IF NOT EXISTS asset (
  asset_id    INT AUTO_INCREMENT PRIMARY KEY,
  asset_name  VARCHAR(100) NOT NULL,
  description VARCHAR(255),
  type        VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS player_asset (
  id        INT AUTO_INCREMENT PRIMARY KEY,
  player_id INT NOT NULL,
  asset_id  INT NOT NULL,
  CONSTRAINT fk_pa_player FOREIGN KEY (player_id) REFERENCES player(player_id),
  CONSTRAINT fk_pa_asset  FOREIGN KEY (asset_id)  REFERENCES asset(asset_id)
);

INSERT INTO player (player_name, full_name, age, level) VALUES
('Player 1', 'Nguyen Van A', 20, 10),
('Player 2', 'Tran Van B', 19, 3),
('Player 3', 'Le Thi C', 23, 10);

INSERT INTO asset (asset_name, description, type) VALUES
('Hero 1', 'Strong hero', 'Hero'),
('Hero 2', 'Quick hero',  'Hero');

INSERT INTO player_asset (player_id, asset_id) VALUES
(1,1),(2,2),(3,1);
```

Chạy script bằng MySQL Workbench / phpMyAdmin / CLI.

---

## 4) Backend – Azure Functions

### 4.1 Cấu hình `local.settings.json`

**File mẫu:** `backend/local.settings.json.sample`  
> Sao chép file này thành `local.settings.json` và sửa user/password MySQL.

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "MySqlConnection": "Server=localhost;Database=battlegame;User Id=root;Password=;"
  },
  "Host": {
    "CORS": "http://localhost:3000",
    "CORSCredentials": false
  }
}
```

> **Lưu ý:** Không commit `local.settings.json` thật lên repo.

### 4.2 Chạy backend

```bash
cd backend
func start
```

Kỳ vọng hiển thị:

```
Functions:
  registerplayer:    [POST] http://localhost:7071/api/registerplayer
  createasset:       [POST] http://localhost:7071/api/createasset
  assignasset:       [POST] http://localhost:7071/api/assignasset
  getassetsbyplayer: [GET]  http://localhost:7071/api/getassetsbyplayer
```

### 4.3 Test nhanh (cURL)

```bash
# tạo player
curl -X POST http://localhost:7071/api/registerplayer   -H "Content-Type: application/json"   -d "{"PlayerName":"Player X","FullName":"Demo","Age":21,"Level":5}"

# tạo asset
curl -X POST http://localhost:7071/api/createasset   -H "Content-Type: application/json"   -d "{"AssetName":"Hero X","Description":"Test","Type":"Hero"}"

# gán asset cho player (bonus)
curl -X POST http://localhost:7071/api/assignasset   -H "Content-Type: application/json"   -d "{"PlayerId":1,"AssetId":1}"

# xem report
curl http://localhost:7071/api/getassetsbyplayer
```

---

## 5) Frontend – React

### 5.1 Cài và chạy dev

```bash
cd battlegame-fe
npm install
npm start
```

**Proxy dev (khuyến nghị):** trong `battlegame-fe/package.json` có:
```json
"proxy": "http://localhost:7071"
```
Lúc dev, code FE gọi `/api/getassetsbyplayer` (đường dẫn tương đối) sẽ tự proxy sang Functions.  
> Nếu không dùng proxy: đặt hằng số `API_BASE = "http://localhost:7071"` trong `App.js`.

### 5.2 Gọi API (ví dụ rút gọn trong `src/App.js`)
```js
const API_BASE = process.env.REACT_APP_API_BASE || ""; // "" dùng proxy

const res = await fetch(`${API_BASE}/api/getassetsbyplayer`, {
  headers: { Accept: "application/json" },
  cache: "no-store"
});
const json = await res.json();
setData(Array.isArray(json) ? json : []);
```

---

## 6) Deploy (tùy chọn)

### 6.1 Deploy Function App (Azure)
- Tạo **Function App** (.NET 8 Isolated).
- App Settings: `MySqlConnection`.
- Tab **CORS**: thêm origin của FE (VD: `https://<your-fe>.azurestaticapps.net`).
- Publish từ VS Code/Azure Tools.

### 6.2 Deploy Static Web Apps (React)
```bash
cd battlegame-fe
npm run build
```
Deploy folder `build/` lên Azure Static Web Apps (hoặc hosting tĩnh khác).  
Đặt env **`REACT_APP_API_BASE`** = `https://<your-function>.azurewebsites.net`.

---

## 7) Troubleshooting

- **FE báo `Unexpected token '<' ...`**: đang nhận **HTML** thay vì JSON.  
  → Dùng **URL tuyệt đối** `http://localhost:7071` **hoặc** thêm `proxy` trong `package.json` và **restart** `npm start`.
- **CORS** local: phải có `Host.CORS` trong `local.settings.json`.  
- **Thiếu `HttpTriggerAttribute`/`AuthorizationLevel`**: cài gói
  `Microsoft.Azure.Functions.Worker.Extensions.Http (>= 3.1.0)`; **không** dùng gói `*.AspNetCore` cho isolated.
- **Lỗi khóa ngoại khi insert `player_asset`**: kiểm tra `player_id`, `asset_id` tồn tại.

---

## 8) Bằng chứng/Minh họa nên kèm (evidence/)
- `api-success.png`: màn hình `func start` liệt kê đủ 4 endpoint.
- `db-join.png`: ảnh kết quả truy vấn JOIN trong MySQL.
- `fe-table.png`: ảnh FE hiển thị bảng.
- (Cloud) `azure-func-portal.png`, `swa-fe.png`.

---

## 9) Ghi chú bảo mật

- Không commit `local.settings.json` thật. Dùng file mẫu `.sample`.
- Khi lên cloud, cân nhắc đổi trigger các API sang `AuthorizationLevel.Function` và dùng **function key**.

---

## 10) Giấy phép

Mã nguồn phục vụ mục đích học tập/demo.
