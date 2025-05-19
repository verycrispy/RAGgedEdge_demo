using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using LMStudioClient.Model;
using Microsoft.Extensions.Options;

namespace LMStudioClient;
public class LmStudioClient
{
    private readonly HttpClient _httpClient;
    private readonly LMStudioConfig _configuration;

    public LmStudioClient(IOptions<LMStudioConfig> config)
    {
        _configuration = config.Value;
        _httpClient = new HttpClient { BaseAddress = new Uri(_configuration.Endpoint) };
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<float[]?> GetEmbeddingAsync(string inputText)
    {
        var requestBody = new
        {
            model = _configuration.Embeddings,
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

    public async Task<EmbeddingItem[]?> GetEmbeddingsAsync(string[] inputTexts)
    {
        var requestBody = new
        {
            model = _configuration.Embeddings,
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
        ChatRequest message = new ChatRequest()
        {
            Model = _configuration.Llm,
            Temperature = 0.7f,
            Messages = messages,
            Stream = false
        };

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

    public async IAsyncEnumerable<string> StreamChatCompletionsAsync(params Message[] messages)
    {
        Debugger.Break();
        ChatRequest message = new ChatRequest()
        {
            Model = _configuration.Llm,
            Temperature = 0.7f,
            Messages = messages,
            Stream = true
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/v1/chat/completions")
        {
            Content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith("data: "))
                line = line.Substring("data: ".Length);

            if (line == "[DONE]") break;

            var chunk = JsonSerializer.Deserialize<StreamChunk>(line);
            var contentPiece = chunk?.Choices?.FirstOrDefault()?.Delta?.Content;
            if (!string.IsNullOrEmpty(contentPiece))
            {
                yield return contentPiece;
            }
        }
    }


    private float[]? DeserializeEmbedding(string jsonResponse)
    {
        var embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(jsonResponse);
        return embeddingResponse?.Data?.FirstOrDefault()?.Embedding?.ToArray();
    }

    private List<EmbeddingItem> GetEmbeddingItems(string jsonResponse)
    {
        var embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(jsonResponse);

        // Map each item in the response data to an EmbeddingItem
        return embeddingResponse?.Data?
            .Select(data => new EmbeddingItem(data.Embedding.ToArray(), data.Index))
            .ToList() ?? new List<EmbeddingItem>();
    }

    private List<Message> GetCompletionItems(string jsonResponse)
    {
        var chatResponse = JsonSerializer.Deserialize<ChatResponse>(jsonResponse);

        return chatResponse?.Choices?
            .Select(choice => choice.Message)
            .ToList() ?? new List<Message>();
    }
}
