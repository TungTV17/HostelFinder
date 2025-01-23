using System.Text.Json.Serialization;

namespace HostelFinder.Application.DTOs.ChatAI.Request;

public class OpenAiChatRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "gpt-3.5-turbo";

    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.7;
}