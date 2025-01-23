using HostelFinder.Application.DTOs.ChatAI.Request;
using HostelFinder.Application.DTOs.ChatAI.Response;
using HostelFinder.Application.DTOs.Post.Requests;
using HostelFinder.Application.DTOs.Post.Responses;

namespace HostelFinder.Application.Interfaces.IServices;

public interface IOpenAiService
{
    Task<OpenAiChatResponse> GenerateAsync (OpenAiChatRequest request);
    Task<PostGenerationResponse> GeneratePostDescriptionsAsync(PostGenerationRequest request);
}