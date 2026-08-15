using System.Net;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.Abstractions.Models;

namespace NotificationServices.Email;

internal sealed class DefaultEmailTemplateProvider : IEmailTemplateProvider
{
    public ValueTask<EmailTemplate> GetOtpTemplateAsync(
        EmailOtp otp,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(otp);
        cancellationToken.ThrowIfCancellationRequested();

        var subject = string.IsNullOrWhiteSpace(otp.Subject)
            ? "Verification Code"
            : otp.Subject;

        var code = WebUtility.HtmlEncode(otp.Code);
        var body = $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <title>{WebUtility.HtmlEncode(subject)}</title>
            </head>
            <body style="margin:0;padding:0;background:#f5f5f5;">
                <div style="max-width:480px;margin:40px auto;padding:32px;background:#ffffff;border-radius:12px;font-family:Arial,sans-serif;text-align:center;">
                    <h2 style="margin-bottom:24px;">Verification Code</h2>
                    <p style="font-size:15px;color:#555;">Your verification code is:</p>
                    <div style="margin:24px 0;padding:16px;background:#f1f1f1;border-radius:8px;font-size:28px;font-weight:bold;letter-spacing:6px;">{code}</div>
                    <p style="font-size:13px;color:#888;">If you did not request this code, you can safely ignore this email.</p>
                </div>
            </body>
            </html>
            """;

        return ValueTask.FromResult(new EmailTemplate(subject, body));
    }
}
