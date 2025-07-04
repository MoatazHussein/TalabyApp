using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common.Interfaces;
using Talaby.Domain.Entities;

namespace Talaby.Infrastructure.Email;

public class MailService(ILogger<AppUser> logger, IConfiguration configuration) : IMailService
{
    private readonly ILogger<AppUser> _logger = logger;

    public async Task SendEmailAsync(string to, string subject, string body, List<string>? attachmentPaths)
    {
        string Host = configuration["Smtp:Host"]!;
        string Port = configuration["Smtp:Port"]!;
        string Username = configuration["Smtp:Username"]!;
        string Password = configuration["Smtp:Password"]!;

        if (Host is not null && Port is not null && Username is not null && Password is not null)
        {

            try
            {
                var smtpClient = new SmtpClient(Host)
                {
                    Port = int.Parse(Port),
                    Credentials = new NetworkCredential(Username, Password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(Username),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                if (attachmentPaths is not null && attachmentPaths.Count > 0)
                {
                    foreach (var attachmentPath in attachmentPaths)
                    {
                        if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                        {
                            mailMessage.Attachments.Add(new Attachment(attachmentPath));
                        }
                    }
                }

                mailMessage.To.Add(to);

                //smtpClient.UseDefaultCredentials = true;

                await smtpClient.SendMailAsync(mailMessage);


                //return new ResponseResult
                //{
                //    Success = true,
                //    ErrorMessage = ""
                //};
            }

            catch (SmtpException smtpEx)
            {
                // Handle specific SMTP exceptions like invalid credentials, server issues, etc.
                _logger.LogInformation($"Mail-Error : SMTP error: {smtpEx.Message}");

                //return new ResponseResult
                //{
                //    Success = false,
                //    ErrorMessage = $"Mail-Error : SMTP error: {smtpEx.Message}"
                //};


            }
            catch (Exception ex)
            {
                // Handle other exceptions like invalid file paths, etc.
                _logger.LogInformation($"Mail-Error : Error sending emails: {ex.Message}");

                //return new ResponseResult
                //{
                //    Success = false,
                //    ErrorMessage = $"Mail-Error : Error sending emails: {ex.Message}"
                //};

            }

        }

        //return new ResponseResult
        //{
        //    Success = true,
        //    ErrorMessage = $"Invalid data for the mail"
        //};
    }
}
