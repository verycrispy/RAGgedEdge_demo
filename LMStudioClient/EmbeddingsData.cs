using System.Text.Json.Serialization;

public class EmbeddingData
{
    [JsonPropertyName("embedding")]
    public List<float> Embedding { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }
}