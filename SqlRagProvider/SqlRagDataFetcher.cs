using System.Data;
using System.Text.Json.Nodes;
using SqlRagProvider.Model;

namespace SqlRagProvider;

public class SqlRagDataFetcher
{

    public static async Task<WikiPageResult[]> GetDatabaseResults(float[] vectors)
    {
        var vectorsTable = new DataTable();
        vectorsTable.Columns.Add("VectorValueId", typeof(int));
        vectorsTable.Columns.Add("VectorValue", typeof(float));

        for (var i = 0; i < vectors.Length; i++)
        {
            vectorsTable.Rows.Add(i + 1, vectors[i]);
        }

        var results = await SqlVectorExecuter.RunVectorSearchStoredProcedure(vectors);
        return results.ToArray();
    }
}
