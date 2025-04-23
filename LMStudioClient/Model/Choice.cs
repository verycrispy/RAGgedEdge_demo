using System.Text.Json.Serialization;

namespace LMStudioClient.Model;

public class Choice
{
    [JsonPropertyName("delta")]
    public Delta Delta { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("logprobs")]
    public object LogProbs { get; set; }

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }
}
