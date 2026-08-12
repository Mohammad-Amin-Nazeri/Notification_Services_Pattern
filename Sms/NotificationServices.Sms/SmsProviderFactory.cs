using NotificationServices.Sms.Abstractions.Enums;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Providers;

namespace NotificationServices.Sms;

public sealed class SmsProviderFactory(
    ISmsProviderOptionsProvider optionsProvider,
    IHttpClientFactory httpClientFactory) : ISmsProviderFactory
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

        if (!Enum.TryParse<SmsProviderType>(
                options.ProviderType,
                ignoreCase: true,
                out var providerType))
        {
            throw new NotSupportedException(
                $"Unknown SMS provider type: '{options.ProviderType}'.");
        }

        return providerType switch
        {
            SmsProviderType.Melipayamak =>
                new MelipayamakSmsProvider(
                    options,
                    httpClientFactory.CreateClient(
                        nameof(MelipayamakSmsProvider))),

            _ => throw new NotSupportedException(
                $"SMS provider '{providerType}' is not implemented.")
        };
    }
}