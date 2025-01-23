using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HostelFinder.Infrastructure.Services;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3Service(IConfiguration configuration)
    {
        var awsAccessKey = configuration["AWS:AccessKey"];
        var awsSecretKey = configuration["AWS:SecretKey"];
        var region = configuration["AWS:Region"];
        _bucketName = configuration["AWS:BucketName"];

        var awsCredentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
        _s3Client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.GetBySystemName(region));
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File không hợp lệ.");

        var fileName =
            $"{Path.GetFileNameWithoutExtension(file.FileName)}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        using (var newMemoryStream = new MemoryStream())
        {
            file.CopyTo(newMemoryStream);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = fileName,
                BucketName = _bucketName,
                ContentType = file.ContentType,
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
        }
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        var fileName = Path.GetFileName(fileUrl);
        var deleteObjectRequest = new Amazon.S3.Model.DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName
        };

        await _s3Client.DeleteObjectAsync(deleteObjectRequest);
    }
    
    public async Task<string> UploadFileFromUrlAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl))
            throw new ArgumentException("URL không hợp lệ.");

        // Tải file từ URL
        using (var httpClient = new HttpClient())
        using (var response = await httpClient.GetAsync(fileUrl))
        {
            if (!response.IsSuccessStatusCode)
                throw new Exception("Không thể tải file từ URL.");

            var fileStream = await response.Content.ReadAsStreamAsync();

            // Tạo tên file cho file mới upload lên S3
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileUrl)}";

            // Tạo yêu cầu upload file lên S3
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = fileName,
                BucketName = _bucketName,
                ContentType = "image/jpeg", // Có thể thay đổi loại content type tùy thuộc vào loại file bạn tải lên
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            // Trả về URL của file trên S3
            return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
        }
    }

}