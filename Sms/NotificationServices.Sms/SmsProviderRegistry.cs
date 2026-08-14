using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms;

public sealed class SmsProviderRegistry(
    IServiceProvider serviceProvider,
    IReadOnlyDictionary<string, Type> registrations) : ISmsProviderRegistry
{
    public bool Contains(string providerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);
        return registrations.ContainsKey(providerName);
    }

    public ISmsProvider CreateProvider(
        string providerName,
        SmsProviderOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);
        ArgumentNullException.ThrowIfNull(options);

        if (!registrations.TryGetValue(providerName, out var providerType))
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
