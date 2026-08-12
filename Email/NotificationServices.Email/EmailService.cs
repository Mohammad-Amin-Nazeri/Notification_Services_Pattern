using System.Net;
using System.Net.Mail;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.Abstractions.Models;

namespace NotificationServices.Email;

/// <summary>Sends email via SMTP using settings resolved from <see cref="IEmailProviderOptionsProvider"/>.</summary>
public class EmailService(IEmailProviderOptionsProvider optionsProvider) : IEmailService
{
    public async Task<EmailResult> SendAsync(EmailRequest request)
    {
        var options = await optionsProvider.GetSettingAsync();
        try
        {
            using var client = BuildSmtpClient(options);
            using var message = BuildMailMessage(options, request);
            await client.SendMailAsync(message);
            return new EmailResult { IsSuccess = true };
        }
        catch (Exception ex)
        {
            return new EmailResult { IsSuccess = false, Message = ex.Message };
        }
    }

    public async Task<EmailResult> SendBulkAsync(IReadOnlyCollection<EmailRequest> requests)
    {
        var options = await optionsProvider.GetSettingAsync();
        try
        {
            using var client = BuildSmtpClient(options);
            foreach (var request in requests)
            {
                using var message = BuildMailMessage(options, request);
                await client.SendMailAsync(message);
            }
            return new EmailResult { IsSuccess = true };
        }
        catch (Exception ex)
        {
            return new EmailResult { IsSuccess = false, Message = ex.Message };
        }
    }

    private static SmtpClient BuildSmtpClient(EmailProviderOptions options) =>
        new(options.Host, options.Port)
        {
            EnableSsl = options.EnableSsl,
            Credentials = new NetworkCredential(options.Username, options.Password),
        };

    // NOTE: this template mirrors the original OTP-style email used in the source project.
    // If you need per-call subject/body customization, extend EmailRequest with optional
    // Subject/Body properties and fall back to this template when they are not provided.
    private static MailMessage BuildMailMessage(EmailProviderOptions options, EmailRequest request) =>
        new()
        {
            From = new MailAddress(options.FromAddress, options.FromName),
            Subject = "کد تایید",
            Body = BuildHtmlBody(request.Text),
            IsBodyHtml = true,
            To = { request.To },
        };

    private static string BuildHtmlBody(string text) => $"""
        <div dir="rtl" style="font-family: Tahoma, Arial, sans-serif; font-size: 15px; color: #222; max-width: 480px; margin: auto;">
            <p>کد تایید شما : {text}</p>
            <hr style="border: none; border-top: 1px solid #eee;" />
        </div>
        """;
}
