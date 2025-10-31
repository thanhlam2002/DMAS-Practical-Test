import React, { useEffect, useState } from "react";
import "./App.css";

// Ưu tiên dùng env khi deploy, còn local dev dùng proxy (mục 2)
const API_BASE = "http://localhost:7071";

function App() {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const load = async () => {
    setLoading(true);
    setError("");
    try {
      const res = await fetch(`${API_BASE}/api/getassetsbyplayer`);
      if (!res.ok) {
        const txt = await res.text();
        throw new Error(`HTTP ${res.status}: ${txt}`);
      }
      const json = await res.json();
      setData(Array.isArray(json) ? json : []);
    } catch (e) {
      console.error(e);
      setError(e.message || "Fetch failed");
      setData([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <div className="App">
      <h2>🕹️ Battle Game - Player Assets Report</h2>

      <div style={{ marginBottom: 12 }}>
        <button onClick={load} disabled={loading}>
          {loading ? "Loading..." : "Reload"}
        </button>
        {error && <span style={{ color: "crimson", marginLeft: 12 }}>{error}</span>}
      </div>

      <table>
        <thead>
          <tr>
            <th>No</th>
            <th>Player Name</th>
            <th>Level</th>
            <th>Age</th>
            <th>Asset Name</th>
          </tr>
        </thead>
        <tbody>
          {data.length > 0 ? (
            data.map((item) => (
              <tr key={item.No}>
                <td>{item.No}</td>
                <td>{item.PlayerName}</td>
                <td>{item.Level}</td>
                <td>{item.Age}</td>
                <td>{item.AssetName}</td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="5">{loading ? "Loading..." : "No data available"}</td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}

export default App;
