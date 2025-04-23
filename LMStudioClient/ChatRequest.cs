using System.Text.Json.Serialization;

namespace LMStudioClient;

public class ChatRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; }
    [JsonPropertyName("messages")]
    public Message[] Messages { get; set; }
    [JsonPropertyName("temperature")]
    public float Temperature { get; set; }
    [JsonPropertyName("stream")]
    public bool Stream { get; set; }
}