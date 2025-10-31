using System.Net;
using System.Text.Json;
using System.IO;
using MySql.Data.MySqlClient;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

public static class RegisterPlayer
{
    [Function("registerplayer")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        FunctionContext context)
    {
        var logger = context.GetLogger("registerplayer");
        HttpResponseData response = req.CreateResponse(); // ✅ hợp lệ

        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var player = JsonSerializer.Deserialize<Player>(body);

            string connStr = Environment.GetEnvironmentVariable("MySqlConnection");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            string sql = "INSERT INTO player (player_name, full_name, age, level) VALUES (@name, @full, @age, @lvl)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", player.PlayerName);
            cmd.Parameters.AddWithValue("@full", player.FullName);
            cmd.Parameters.AddWithValue("@age", player.Age);
            cmd.Parameters.AddWithValue("@lvl", player.Level);
            await cmd.ExecuteNonQueryAsync();

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteStringAsync("Player registered successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync("Error: " + ex.Message);
        }

        return response;
    }

    private class Player
    {
        public string PlayerName { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public int Level { get; set; }
    }
}
