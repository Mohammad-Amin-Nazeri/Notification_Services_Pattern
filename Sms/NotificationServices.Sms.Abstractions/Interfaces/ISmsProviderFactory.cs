using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.Abstractions.Interfaces;

/// <summary>
/// Resolves the concrete <see cref="ISmsProvider"/> to use, based on the
/// currently configured <see cref="SmsProviderOptions.ProviderType"/>.
/// </summary>
public interface ISmsProviderFactory
{
    Task<ISmsProvider> GetProviderAsync();
}
