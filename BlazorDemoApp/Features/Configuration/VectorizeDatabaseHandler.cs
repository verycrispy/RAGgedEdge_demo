using LMStudioClient;
using LMStudioClient.Model;
using Microsoft.Extensions.Options;
using SqlRagProvider;

namespace BlazorDemoApp.wwwroot.Features.Configuration;

public class VectorizeDatabaseHandler
{
    private readonly SqlDataVectorizer _dataVectorizer;

    public VectorizeDatabaseHandler(SqlDataVectorizer sqlDataVetorizer)
    {
        _dataVectorizer = sqlDataVetorizer;
    }

    public async Task HandleAsync(HttpContext context)
    {
        //await _dataVectorizer.VectorizeEntities();


        await foreach (var part in _dataVectorizer.VectorizeEntities())
        {
            await context.Response.WriteAsync(part + "\n");
            await context.Response.Body.FlushAsync();
        }

        var answer = ""; // await _lmClient.GetChatCompletionsAsync(messages);

        await context.Response.WriteAsync(answer);
    }

}