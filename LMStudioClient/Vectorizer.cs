using Microsoft.Extensions.Configuration;

namespace LMStudioClient;

public class Vectorizer
{
    public static async Task<float[]> VectorizeQuestion(string question)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        LmStudioClient lmStudioClient = new LmStudioClient(config["LMStudio:Endpoint"]);
        
        var result = await lmStudioClient.GetEmbeddingAsync(question, config["LmStudio:Embeddings"]);
        return result;
    }
}