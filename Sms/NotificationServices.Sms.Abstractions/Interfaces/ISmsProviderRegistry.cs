using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.Abstractions.Interfaces;

/// <summary>
/// Resolves and creates SMS providers by their configured provider key.
/// Implementations must resolve providers at runtime so different tenants or licenses
/// can select different providers without changing application registrations.
/// </summary>
public interface ISmsProviderRegistry
{
    bool Contains(string providerName);

    ISmsProvider CreateProvider(
        string providerName,
        SmsProviderOptions options);
}
