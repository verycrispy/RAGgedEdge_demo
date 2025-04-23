using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SqlRagProvider.Model;

namespace SqlRagProvider;

internal class SqlVectorExecuter
{
    public static async Task<IEnumerable<WikiPageResult>> RunVectorSearchStoredProcedure(object vectors)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        using var conn = new SqlConnection(config["AppConfig:ConnectionString"]);
        await conn.OpenAsync();

        using var cmd = new SqlCommand("RunWikiVectorSearch", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = 60 * 30;
        cmd.Parameters.AddWithValue("@Vectors", vectors);

        using var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        var results = new List<WikiPageResult>();
        while (await rdr.ReadAsync())
        {
            results.Add(new WikiPageResult
            {
                Id = rdr.GetInt32(0),
                Title = rdr.GetString(1),
                Subject = rdr.GetString(2),
                Content = rdr.GetString(3),
                SimilarityScore = rdr.GetDouble(4)
            });
        }

        await rdr.CloseAsync();
        return results;
    }
}
