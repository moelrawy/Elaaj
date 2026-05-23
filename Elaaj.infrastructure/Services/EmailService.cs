using Elaaj.Application.Interfaces.Services;
using Elaaj.Application.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Elaaj.infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            // Note: UseDefaultCredentials MUST be false before setting the Credentials property
            var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
            {
                Port = _emailSettings.SmtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false, // 👈 Critical for Gmail
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.Password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation($"Email successfully sent to {toEmail}");
        }
        catch (Exception ex)
        {
            // If the email fails, we log the error to the console/Debugger so you can see why
            _logger.LogError($"Failed to send email to {toEmail}: {ex.Message}");
            throw; // Option: Remove this if you don't want the user registration to fail when email fails
        }
    }
}
