namespace HostelFinder.Infrastructure.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string emailTo, string subject, string body);
    }
}