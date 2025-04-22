using System.Text.Json.Serialization;

namespace LMStudioClient;
public class EmbeddingResponse
{
    [JsonPropertyName("data")]
    public List<EmbeddingData> Data { get; set; }
}