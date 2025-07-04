namespace Talaby.Application.Common.Interfaces;

public interface IMailService
{
    Task SendEmailAsync(string to, string subject, string body, List<string>? attachmentPaths);
}
