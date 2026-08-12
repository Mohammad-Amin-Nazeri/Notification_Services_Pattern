using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.Abstractions.Interfaces;

/// <summary>
/// Provides SMS provider configuration.
/// </summary>
public interface ISmsProviderOptionsProvider
{
    Task<SmsProviderOptions> GetSettingAsync(
        CancellationToken cancellationToken = default);
}