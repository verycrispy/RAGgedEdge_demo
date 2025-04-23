using System.Text.Json.Serialization;

namespace LMStudioClient.Model;

public class StreamChunk
{
    [JsonPropertyName("choices")]
    public Choice[] Choices { get; set; }
}
