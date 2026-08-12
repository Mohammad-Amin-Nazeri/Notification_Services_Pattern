using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.Abstractions.Interfaces;

/// <summary>
/// Resolves the configured SMS provider.
/// </summary>
public interface ISmsProviderFactory
{
    Task<ISmsProvider> GetProviderAsync(
        CancellationToken cancellationToken = default);
}