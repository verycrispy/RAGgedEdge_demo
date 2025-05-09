using System.Diagnostics;
using Dapper;
using LMStudioClient;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SqlRagProvider.Model;

namespace SqlRagProvider;
public class SqlDataVectorizer
{

    private readonly IConfiguration _config;
    private readonly LmStudioClient _lmClient;

    public SqlDataVectorizer(LmStudioClient lmStudioClient, IConfiguration config)
    {
        _lmClient = lmStudioClient;
        _config = config;
    }

    private SqlConnection CreateConnection()
    {
        return new SqlConnection(_config["ConnectionString"]);
    }

    public async IAsyncEnumerable<string> VectorizeEntities()
    {
        Debugger.Break();
        using var connection = CreateConnection();
        var sql = "SELECT Id, Title, Subject, Content FROM Wiki";
        var wikipages = await connection.QueryAsync<WikiPageResult>(sql);

        yield return $"Vectorizing {wikipages.Count()} wikipages(s)";

        const int BatchSize = 50;
        var itemCount = 0;

        for (var i = 0; i < wikipages.Count(); i += BatchSize)
        {
            // Retrieve the next batch of documents
            var wikipageBatch = wikipages.Skip(i).Take(BatchSize).ToArray();
            foreach (var wikipage in wikipageBatch)
            {
                yield return $"{++itemCount,5}: Vectorizing entity - {wikipage.Title} (ID {wikipage.Id})";
            }

            var embeddings = await this.GenerateEmbeddings(wikipageBatch);

            await this.SaveVectors(wikipageBatch, embeddings);
        }

        yield return $"Generated and embedded vectors for {itemCount} document(s)";
    }

    private async Task<IReadOnlyList<EmbeddingItem>> GenerateEmbeddings(WikiPage[] documents)
    {
        var input = documents.Select(d =>  System.Text.Json.JsonSerializer.Serialize(d)).ToArray();

        var embeddings = await _lmClient.GetEmbeddingsAsync(input);

        Debug.WriteLine(embeddings.Count());

        return embeddings;
    }

    private async Task SaveVectors(WikiPage[] documents, IReadOnlyList<EmbeddingItem> embeddings)
    {
        Debug.WriteLine("Saving vectors");

        using var connection = new SqlConnection(_config["ConnectionString"]);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        try
        {
            var sql = @"
            INSERT INTO WikiVector (WikiId, VectorValueId, VectorValue)
            VALUES (@WikiId, @VectorValueId, @VectorValue);";

            var parameters = new List<dynamic>();

            for (var i = 0; i < documents.Length; i++)
            {
                var wikiId = documents[i].Id;
                var vectors = embeddings[i].Embedding;

                var vectorValueId = 1;
                foreach (var vectorValue in vectors.ToArray())
                {
                    parameters.Add(new
                    {
                        WikiId = wikiId,
                        VectorValueId = vectorValueId++,
                        VectorValue = vectorValue
                    });
                }
            }

            await connection.ExecuteAsync(sql, parameters, transaction);

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}
