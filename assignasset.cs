using System.Net;
using System.Text.Json;
using System.IO;
using MySql.Data.MySqlClient;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public static class AssignAsset
{
    [Function("assignasset")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        FunctionContext context)
    {
        var logger = context.GetLogger("assignasset");
        var response = req.CreateResponse();

        try
        {
            // Đọc body JSON
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<PlayerAsset>(body);

            string connStr = Environment.GetEnvironmentVariable("MySqlConnection");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            // Thêm mối quan hệ player - asset
            string sql = "INSERT INTO player_asset (player_id, asset_id) VALUES (@playerId, @assetId)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@playerId", data.PlayerId);
            cmd.Parameters.AddWithValue("@assetId", data.AssetId);
            await cmd.ExecuteNonQueryAsync();

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteStringAsync("Asset assigned to player successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error assigning asset");
            response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync($"Error: {ex.Message}");
        }

        return response;
    }

    private class PlayerAsset
    {
        public int PlayerId { get; set; }
        public int AssetId { get; set; }
    }
}
