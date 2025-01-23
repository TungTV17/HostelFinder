using System.Text.Json.Serialization;

namespace HostelFinder.Application.DTOs.ChatAI.Response;

public class OpenAiChatResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("created")]
    public int Created { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("usage")]
    public Usage Usage { get; set; }

    [JsonPropertyName("choices")]
    public List<ChoiceChat> Choices { get; set; }
}