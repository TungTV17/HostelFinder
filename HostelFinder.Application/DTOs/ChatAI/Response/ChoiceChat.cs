using System.Text.Json.Serialization;
using HostelFinder.Application.DTOs.ChatAI.Request;

namespace HostelFinder.Application.DTOs.ChatAI.Response;

public class ChoiceChat
{
    [JsonPropertyName("message")]
    public Message Message { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }
}