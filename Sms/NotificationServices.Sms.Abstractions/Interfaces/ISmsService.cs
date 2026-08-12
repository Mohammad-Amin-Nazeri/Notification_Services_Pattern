using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.Abstractions.Interfaces;


/// <summary>
/// High-level entry point for SMS notifications.
/// </summary>
public interface ISmsService
{
    /// <summary>
    /// Sends a general-purpose SMS message.
    /// </summary>
    Task<SmsResult> SendMessageAsync(
        SmsMessage message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a one-time password SMS.
    /// </summary>
    Task<SmsResult> SendOtpAsync(
        SmsOtp otp,
        CancellationToken cancellationToken = default);
}