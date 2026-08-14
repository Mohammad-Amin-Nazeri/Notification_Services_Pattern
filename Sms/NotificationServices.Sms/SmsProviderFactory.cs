using NotificationServices.Sms.Abstractions.Interfaces;

namespace NotificationServices.Sms;

public sealed class SmsProviderFactory(
    ISmsProviderOptionsProvider optionsProvider,
    ISmsProviderRegistry providerRegistry) : ISmsProviderFactory
{
    public async Task<ISmsProvider> GetProviderAsync(
        CancellationToken cancellationToken = default)
    {
        var options = await optionsProvider.GetSettingAsync(
            cancellationToken);

        if (string.IsNullOrWhiteSpace(options.ProviderType))
        {
            throw new InvalidOperationException(
                "SMS provider type is not configured.");
        }

        return providerRegistry.CreateProvider(
            options.ProviderType,
            options);
    }
}
