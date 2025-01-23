using System.Text.Json.Serialization;

namespace HostelFinder.Application.DTOs.ChatAI.Request;

public class Message
{
    [JsonPropertyName("role")]
    public string Role { get; set; } // "system", "user", hoặc "assistant"

    [JsonPropertyName("content")]
    public string Content { get; set; }
}