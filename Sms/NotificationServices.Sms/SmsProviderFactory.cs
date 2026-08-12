using NotificationServices.Sms.Abstractions.Enums;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Providers;
using Microsoft.Extensions.Configuration;

namespace NotificationServices.Sms;

/// <summary>
/// Resolves the <see cref="ISmsProvider"/> matching the configured
/// <see cref="Abstractions.Models.SmsProviderOptions.ProviderType"/>.
/// Add a new arm to the switch expression whenever a new provider is implemented
/// (see <see cref="MelipayamakSmsProvider"/> for a reference implementation).
/// </summary>
public class SmsProviderFactory(
    ISmsProviderOptionsProvider optionsProvider,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : ISmsProviderFactory
{
    public async Task<ISmsProvider> GetProviderAsync()
    {
        var options = await optionsProvider.GetSettingAsync();

        if (!Enum.TryParse<SmsProviderType>(options.ProviderType, ignoreCase: true, out var type))
            throw new NotSupportedException($"Unknown SMS provider type: \"{options.ProviderType}\".");

        return type switch
        {
            SmsProviderType.Melipayamak => new MelipayamakSmsProvider(
                options,
                httpClientFactory.CreateClient(nameof(MelipayamakSmsProvider)),
                configuration["SmsProvider:Melipayamak:BaseUrl"]
                    ?? throw new InvalidOperationException(
                        "Configuration key \"SmsProvider:Melipayamak:BaseUrl\" is missing.")),

            _ => throw new NotSupportedException($"SMS provider \"{type}\" is not implemented yet.")
        };
    }
}
