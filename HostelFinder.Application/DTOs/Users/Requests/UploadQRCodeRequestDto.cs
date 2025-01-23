using Microsoft.AspNetCore.Http;

namespace HostelFinder.Application.DTOs.Users.Requests;

public class UploadQRCodeRequestDto
{
    public IFormFile QRCodeImage { get; set; }
    public string? AccountNumber { get; set; }
        
    public string? BankName { get; set; }
}