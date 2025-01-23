using Microsoft.AspNetCore.Http;

namespace HostelFinder.Application.DTOs.Image.Requests
{
    public class UploadImageRequestDto
    {
        public IFormFile? Image { get; set; }
    }
}
