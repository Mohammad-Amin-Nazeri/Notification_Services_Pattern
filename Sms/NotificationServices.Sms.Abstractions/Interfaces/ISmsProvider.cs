using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.Abstractions.Interfaces;

/// <summary>
/// Represents a concrete SMS gateway (Melipayamak, Kavenegar, Twilio, ...).
/// Implement this to add support for a new gateway; register it in
/// <c>SmsProviderFactory</c> and add a matching value to <c>SmsProviderType</c>.
/// </summary>
public interface ISmsProvider
{
    Task<SmsResult> SendAsync(SmsRequest request);
    Task<SmsResult> SendBulkAsync(IReadOnlyCollection<SmsRequest> requests);
}
