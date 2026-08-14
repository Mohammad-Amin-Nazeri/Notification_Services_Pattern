using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MimeKit;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.Abstractions.Models;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace NotificationServices.Email;

public sealed class EmailService(
    IEmailProviderOptionsProvider optionsProvider,
    ILogger<EmailService>? logger = null) : IEmailService
{
    private readonly ILogger<EmailService> _logger = logger ?? NullLogger<EmailService>.Instance;
    private const string DefaultOtpSubject = "Verification Code";

    public async Task<EmailResult> SendMessageAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        ValidateMessage(message);
        var options = await optionsProvider.GetSettingAsync(cancellationToken);

        try
        {
            var email = BuildMessage(options, message.To, message.Subject, message.Body, message.IsHtml);
            await SendAsync(options, email, cancellationToken);
            _logger.LogInformation("Email notification sent successfully.");
            return EmailResult.Success();
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Email notification was cancelled.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email notification failed.");
            return EmailResult.Failure(ex.Message, ex.GetType().Name);
        }
    }

    public async Task<EmailResult> SendOtpAsync(
        EmailOtp otp,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(otp);
        ValidateOtp(otp);
        var options = await optionsProvider.GetSettingAsync(cancellationToken);

        try
        {
            var subject = string.IsNullOrWhiteSpace(otp.Subject) ? DefaultOtpSubject : otp.Subject;
            var body = BuildOtpBody(otp.Code);
            var email = BuildMessage(options, otp.To, subject, body, isHtml: true);
            await SendAsync(options, email, cancellationToken);
            _logger.LogInformation("Email OTP notification sent successfully.");
            return EmailResult.Success();
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Email OTP notification was cancelled.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email OTP notification failed.");
            return EmailResult.Failure(ex.Message, ex.GetType().Name);
        }
    }

    private static async Task SendAsync(EmailProviderOptions options, MimeMessage message, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();
        var secureSocketOptions = GetSecureSocketOptions(options);
        await client.ConnectAsync(options.Host, options.Port, secureSocketOptions, cancellationToken);

        if (!string.IsNullOrWhiteSpace(options.Username))
            await client.AuthenticateAsync(options.Username, options.Password, cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }

    private static MimeMessage BuildMessage(EmailProviderOptions options, string to, string subject, string body, bool isHtml)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(options.FromName, options.FromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new BodyBuilder
        {
            HtmlBody = isHtml ? body : null,
            TextBody = isHtml ? null : body
        }.ToMessageBody();
        return message;
    }

    private static SecureSocketOptions GetSecureSocketOptions(EmailProviderOptions options)
        => !options.EnableSsl ? SecureSocketOptions.None : options.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

    private static string BuildOtpBody(string code)
        => $"""
            <!DOCTYPE html>
            <html lang="en">
            <head><meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1.0" /><title>Verification Code</title></head>
            <body style="margin:0;padding:0;background:#f5f5f5;">
                <div style="max-width:480px;margin:40px auto;padding:32px;background:#ffffff;border-radius:12px;font-family:Arial,sans-serif;text-align:center;">
                    <h2 style="margin-bottom:24px;">Verification Code</h2>
                    <p style="font-size:15px;color:#555;">Your verification code is:</p>
                    <div style="margin:24px 0;padding:16px;background:#f1f1f1;border-radius:8px;font-size:28px;font-weight:bold;letter-spacing:6px;">{System.Net.WebUtility.HtmlEncode(code)}</div>
                    <p style="font-size:13px;color:#888;">If you did not request this code, you can safely ignore this email.</p>
                </div>
            </body>
            </html>
            """;

    private static void ValidateMessage(EmailMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.To)) throw new ArgumentException("Email recipient is required.", nameof(message));
        if (string.IsNullOrWhiteSpace(message.Subject)) throw new ArgumentException("Email subject is required.", nameof(message));
        if (string.IsNullOrWhiteSpace(message.Body)) throw new ArgumentException("Email body is required.", nameof(message));
    }

    private static void ValidateOtp(EmailOtp otp)
    {
        if (string.IsNullOrWhiteSpace(otp.To)) throw new ArgumentException("Email recipient is required.", nameof(otp));
        if (string.IsNullOrWhiteSpace(otp.Code)) throw new ArgumentException("OTP code is required.", nameof(otp));
    }
}
