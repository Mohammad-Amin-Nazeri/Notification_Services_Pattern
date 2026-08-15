using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.Abstractions.Interfaces;

/// <summary>
/// Resolves and creates SMS providers by their configured provider key.
/// Provider selection is runtime-driven and can be supplied by any application-owned
/// configuration source without changing the notification service API.
/// </summary>
public interface ISmsProviderRegistry
{
    bool Contains(string providerName);

    ISmsProvider CreateProvider(
        string providerName,
        SmsProviderOptions options);
}
