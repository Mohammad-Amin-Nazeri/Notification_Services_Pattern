using Microsoft.Extensions.Configuration;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.ConfigurationProviders;

public sealed class AppSettingsSmsProviderOptionsProvider(
    IConfiguration configuration) : ISmsProviderOptionsProvider
{
    private const string SectionName = "SmsProvider";
    private const string MelipayamakProvider = "Melipayamak";
    private const string BodyIdSetting = "BodyId";

    public Task<SmsProviderOptions> GetSettingAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var section = configuration.GetSection(SectionName);

        if (!section.Exists())
            throw new InvalidOperationException($"Configuration section '{SectionName}' was not found.");

        var options = section.Get<SmsProviderOptions>();

        if (options is null)
            throw new InvalidOperationException($"Could not bind configuration section '{SectionName}'.");

        NormalizeProviderSettings(section, options);
        Validate(options);

        return Task.FromResult(options);
    }

    private static void NormalizeProviderSettings(
        IConfigurationSection section,
        SmsProviderOptions options)
    {
        var providerSettings = section.GetSection("ProviderSettings").Get<Dictionary<string, string>>()
            ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var pair in providerSettings)
            options.ProviderSettings[pair.Key] = pair.Value;

        // Backward-compatible mapping for existing Melipayamak appsettings that still use BodyId directly.
        if (string.Equals(options.ProviderType, MelipayamakProvider, StringComparison.OrdinalIgnoreCase) &&
            !options.ProviderSettings.ContainsKey(BodyIdSetting))
        {
            var legacyBodyId = section[BodyIdSetting];
            if (!string.IsNullOrWhiteSpace(legacyBodyId))
                options.ProviderSettings[BodyIdSetting] = legacyBodyId;
        }
    }

    private static void Validate(SmsProviderOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ProviderType))
            throw new InvalidOperationException("SmsProvider:ProviderType is required.");

        if (string.IsNullOrWhiteSpace(options.Username))
            throw new InvalidOperationException("SmsProvider:Username is required.");

        if (string.IsNullOrWhiteSpace(options.Password))
            throw new InvalidOperationException("SmsProvider:Password is required.");

        if (string.IsNullOrWhiteSpace(options.From))
            throw new InvalidOperationException("SmsProvider:From is required.");

        if (string.IsNullOrWhiteSpace(options.BaseUrl))
            throw new InvalidOperationException("SmsProvider:BaseUrl is required.");

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _))
            throw new InvalidOperationException("SmsProvider:BaseUrl must be a valid absolute URI.");

        if (string.Equals(options.ProviderType, MelipayamakProvider, StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(options.PatternBaseUrl))
                throw new InvalidOperationException("SmsProvider:PatternBaseUrl is required for Melipayamak.");

            if (!Uri.TryCreate(options.PatternBaseUrl, UriKind.Absolute, out _))
                throw new InvalidOperationException("SmsProvider:PatternBaseUrl must be a valid absolute URI.");

            _ = options.GetRequiredProviderSetting(BodyIdSetting);
        }
    }
}
