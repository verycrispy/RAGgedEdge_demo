using Microsoft.Extensions.Configuration;

namespace LMStudioClient;

public class Vectorizer
{
    public static async Task<float[]> VectorizeQuestion(string question)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        LmStudioClient lmStudioClient = new LmStudioClient(config["AppConfig:LmStudio:Endpoint"]);

        var result = await lmStudioClient.GetEmbeddingAsync(question, config["AppConfig:LmStudio:Embeddings"]);
        return result;
    }
}