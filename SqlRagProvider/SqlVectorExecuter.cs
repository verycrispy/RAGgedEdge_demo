using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SqlRagProvider.Model;

namespace SqlRagProvider;

internal class SqlVectorExecuter
{
    public static async Task<IEnumerable<WikiPageResult>> RunVectorSearchStoredProcedure(DataTable vectors)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        using var conn = new SqlConnection(config["ConnectionString"]);
        await conn.OpenAsync();

        var parameters = new DynamicParameters();

        var vectorParam = new SqlParameter("@Vectors", SqlDbType.Structured)
        {
            TypeName = "dbo.VectorsUdt",
            Value = vectors
        };

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "RunWikiVectorSearch";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add(vectorParam);

        using var reader = await cmd.ExecuteReaderAsync();
        var result = new List<WikiPageResult>();

        while (await reader.ReadAsync())
        {
            result.Add(new WikiPageResult
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Subject = reader.GetString(2),
                Content = reader.GetString(3),
                SimilarityScore = reader.GetDouble(4)
            });
        }

        return result;
    }
}
