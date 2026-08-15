using NotificationServices.Email.Abstractions.Models;

namespace NotificationServices.Email.Abstractions.Interfaces;

/// <summary>
/// Provides presentation templates for email notifications.
/// Implementations belong to the consuming application and may load templates
/// from files, resources, databases, or any other application-owned source.
/// </summary>
public interface IEmailTemplateProvider
{
    ValueTask<EmailTemplate> GetOtpTemplateAsync(
        EmailOtp otp,
        CancellationToken cancellationToken = default);
}

public sealed record EmailTemplate(
    string Subject,
    string Body,
    bool IsHtml = true);
