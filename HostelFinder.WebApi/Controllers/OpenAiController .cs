using HostelFinder.Application.DTOs.ChatAI.Request;
using HostelFinder.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpenAiController : ControllerBase
{
    private readonly IOpenAiService _openAiService;

    public OpenAiController(IOpenAiService openAiService)
    {
        _openAiService = openAiService;
    }

    [HttpPost("generate")]
    [Authorize(Roles = "Landlord,Admin")]
    public async Task<IActionResult> Generate([FromBody] OpenAiChatRequest request)
    {
        if (request == null || request.Messages == null || request.Messages.Count == 0)
        {
            return BadRequest("Messages are required.");
        }

        try
        {
            var openAiResponse = await _openAiService.GenerateAsync(request);
            return Ok(openAiResponse);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, $"Error calling OpenAI API: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }
}