using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.Abstractions.Interfaces;

/// <summary>
/// Represents a concrete SMS gateway implementation.
/// </summary>
public interface ISmsProvider
{
    Task<SmsResult> SendMessageAsync(
        SmsMessage message,
        CancellationToken cancellationToken = default);

    Task<SmsResult> SendOtpAsync(
        SmsOtp otp,
        CancellationToken cancellationToken = default);
}