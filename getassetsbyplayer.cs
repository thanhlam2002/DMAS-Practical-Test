using System.Net;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

public static class GetAssetsByPlayer
{
    [Function("getassetsbyplayer")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, // cho phép FE gọi trực tiếp khi dev
        FunctionContext context)
    {
        var logger = context.GetLogger("getassetsbyplayer");
        try
        {
            string connStr = Environment.GetEnvironmentVariable("MySqlConnection");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            const string sql = @"
                SELECT p.player_name, p.level, p.age, a.asset_name
                FROM player p
                JOIN player_asset pa ON p.player_id = pa.player_id
                JOIN asset a ON pa.asset_id = a.asset_id
                ORDER BY p.player_name;";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var result = new List<object>();
            int no = 1;
            while (await reader.ReadAsync())
            {
                result.Add(new
                {
                    No = no++,
                    PlayerName = reader["player_name"],
                    Level = reader["level"],
                    Age = reader["age"],
                    AssetName = reader["asset_name"]
                });
            }

            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(result); // ✅ Content-Type: application/json
            return res;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving assets by player");
            var res = req.CreateResponse(HttpStatusCode.InternalServerError);
            await res.WriteAsJsonAsync(new { error = ex.Message });
            return res;
        }
    }
}
