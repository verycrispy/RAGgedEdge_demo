using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LMStudioClient;

public class Vectorizer
{
    private readonly LmStudioClient _lmClient;
    public Vectorizer(LmStudioClient lmStudioClient)
    {
        _lmClient = lmStudioClient;
    }

    public async Task<float[]> VectorizeQuestion(string question)
    {
        return await _lmClient.GetEmbeddingAsync(question);
    }
}