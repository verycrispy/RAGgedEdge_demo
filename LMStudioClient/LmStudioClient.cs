using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace LMStudioClient;
public class LmStudioClient
{
    private readonly HttpClient _httpClient;

    public LmStudioClient(string baseAddress)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<float[]?> GetEmbeddingAsync(string inputText, string model)
    {
        var requestBody = new {
            model = model,
            input = inputText 
        };

        // Serialize the request body using System.Text.Json
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/v1/embeddings", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        if (responseJson != null)
        {
            return DeserializeEmbedding(responseJson);
        }
        else
        {
            return null;
        }
    }

    public async Task<EmbeddingItem[]?> GetEmbeddingsAsync(string[] inputTexts, string model)
    {
        var requestBody = new 
        { 
            model = model,
            input = inputTexts 
        };

        // Serialize the request body using System.Text.Json
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/v1/embeddings", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        if (responseJson != null)
        {
            return GetEmbeddingItems(responseJson).ToArray();
        }
        else
        {
            return null;
        }

    }

    public async Task<string> GetChatCompletionsAsync(params Message[] messages)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        ChatRequest message = new ChatRequest()
       {
           Model = config["LMStudio:Llm"],
           Temperature = 0.7f,
           Messages = messages,
       };

        // Serialize the request body using System.Text.Json
        var content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/v1/chat/completions", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        if (responseJson != null)
        {
            return GetCompletionItems(responseJson).FirstOrDefault().Content;
        }
        else
        {
            return null;
        }
    }


    private float[]? DeserializeEmbedding(string jsonResponse)
    {
        // Deserialize the JSON response to the EmbeddingResponse class
        var embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(jsonResponse);

        // Extract the embedding array from the first item in the data list
        return embeddingResponse?.Data?.FirstOrDefault()?.Embedding?.ToArray();
    }

    private List<EmbeddingItem> GetEmbeddingItems(string jsonResponse)
    {
        // Deserialize the JSON into an EmbeddingResponse object
        var embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(jsonResponse);

        // Map each item in the response data to an EmbeddingItem
        return embeddingResponse?.Data?
            .Select(data => new EmbeddingItem(data.Embedding.ToArray(), data.Index))
            .ToList() ?? new List<EmbeddingItem>();
    }

    private List<Message> GetCompletionItems(string jsonResponse)
    {
        var embeddingResponse = JsonSerializer.Deserialize<ChatResponse>(jsonResponse);

        return embeddingResponse?.Choices?
            .Select(choice => choice.Message)
            .ToList() ?? new List<Message>();
    }
}
