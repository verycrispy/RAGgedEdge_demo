using SqlRagProvider;

namespace BlazorDemoApp.Features.Configuration;

public class VectorizeDatabaseHandler
{
    private readonly SqlDataVectorizer _dataVectorizer;

    public VectorizeDatabaseHandler(SqlDataVectorizer sqlDataVetorizer)
    {
        _dataVectorizer = sqlDataVetorizer;
    }

    public async Task HandleAsync(HttpContext context)
    {
        await foreach (var part in _dataVectorizer.VectorizeEntities())
        {
            await context.Response.WriteAsync(part + "\n");
            await context.Response.Body.FlushAsync();
        }
    }

}