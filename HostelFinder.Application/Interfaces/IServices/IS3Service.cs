using Microsoft.AspNetCore.Http;

public interface IS3Service
{
    public Task<string> UploadFileAsync(IFormFile file);
    Task DeleteFileAsync(string fileUrl);
    public Task<string> UploadFileFromUrlAsync(string fileUrl);
}