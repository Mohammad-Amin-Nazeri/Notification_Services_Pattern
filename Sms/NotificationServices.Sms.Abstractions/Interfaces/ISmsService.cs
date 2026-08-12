using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.Abstractions.Interfaces;

/// <summary>
/// High-level entry point for sending SMS messages. Application code should depend
/// on this interface only - the underlying gateway/provider is an implementation detail.
/// </summary>
public interface ISmsService
{
    Task<SmsResult> SendAsync(SmsRequest request);
    Task<SmsResult> SendBulkAsync(IReadOnlyCollection<SmsRequest> requests);
}
