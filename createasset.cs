using System.Net;
using System.Text.Json;
using System.IO;
using MySql.Data.MySqlClient;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

public static class CreateAsset
{
    [Function("createasset")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        FunctionContext context)
    {
        var logger = context.GetLogger("createasset");
        var response = req.CreateResponse();

        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var asset = JsonSerializer.Deserialize<Asset>(body);

            string connStr = Environment.GetEnvironmentVariable("MySqlConnection");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            string sql = @"INSERT INTO asset (asset_name, description, type)
                           VALUES (@name, @desc, @type)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", asset.AssetName);
            cmd.Parameters.AddWithValue("@desc", asset.Description);
            cmd.Parameters.AddWithValue("@type", asset.Type);
            await cmd.ExecuteNonQueryAsync();

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteStringAsync("Asset created successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating asset");
            response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync($"Error: {ex.Message}");
        }

        return response;
    }

    private class Asset
    {
        public string AssetName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}
