using NotificationServices.Email.Abstractions.Models;

namespace NotificationServices.Email.Abstractions.Interfaces;

/// <summary>
/// Provides high-level email notification operations.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a general-purpose email message.
    /// </summary>
    Task<EmailResult> SendMessageAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a one-time password email.
    /// </summary>
    Task<EmailResult> SendOtpAsync(
        EmailOtp otp,
        CancellationToken cancellationToken = default);
}