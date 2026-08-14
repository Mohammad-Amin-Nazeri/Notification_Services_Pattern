using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms;

public sealed class SmsProviderRegistry(
    IServiceProvider serviceProvider,
    IEnumerable<SmsProviderRegistration> registrations) : ISmsProviderRegistry
{
    private readonly IReadOnlyDictionary<string, Type> _registrations =
        registrations.ToDictionary(
            registration => registration.Name,
            registration => registration.ProviderType,
            StringComparer.OrdinalIgnoreCase);

    public bool Contains(string providerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);
        return _registrations.ContainsKey(providerName);
    }

    public ISmsProvider CreateProvider(
        string providerName,
        SmsProviderOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);
        ArgumentNullException.ThrowIfNull(options);

        if (!_registrations.TryGetValue(providerName, out var providerType))
        {
            throw new NotSupportedException(
                $"SMS provider '{providerName}' is not registered.");
        }

        var provider = ActivatorUtilities.CreateInstance(
            serviceProvider,
            providerType,
            options);

        return provider as ISmsProvider
            ?? throw new InvalidOperationException(
                $"Registered SMS provider '{providerName}' must implement {nameof(ISmsProvider)}.");
    }
}
