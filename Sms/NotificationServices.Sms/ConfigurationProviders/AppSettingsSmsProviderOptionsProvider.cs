using Microsoft.Extensions.Configuration;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.ConfigurationProviders;

public sealed class AppSettingsSmsProviderOptionsProvider(
    IConfiguration configuration) : ISmsProviderOptionsProvider
{
    private const string SectionName = "SmsProvider";

    public Task<SmsProviderOptions> GetSettingAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var section = configuration.GetSection(SectionName);

        if (!section.Exists())
        {
            throw new InvalidOperationException(
                $"Configuration section '{SectionName}' was not found.");
        }

        var options = section.Get<SmsProviderOptions>();

        if (options is null)
        {
            throw new InvalidOperationException(
                $"Could not bind configuration section '{SectionName}'.");
        }

        Validate(options);

        return Task.FromResult(options);
    }

    private static void Validate(SmsProviderOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ProviderType))
            throw new InvalidOperationException(
                "SmsProvider:ProviderType is required.");

        if (string.IsNullOrWhiteSpace(options.Username))
            throw new InvalidOperationException(
                "SmsProvider:Username is required.");

        if (string.IsNullOrWhiteSpace(options.Password))
            throw new InvalidOperationException(
                "SmsProvider:Password is required.");

        if (string.IsNullOrWhiteSpace(options.From))
            throw new InvalidOperationException(
                "SmsProvider:From is required.");

        if (string.IsNullOrWhiteSpace(options.BaseUrl))
            throw new InvalidOperationException(
                "SmsProvider:BaseUrl is required.");

        if (string.IsNullOrWhiteSpace(options.PatternBaseUrl))
            throw new InvalidOperationException(
                "SmsProvider:PatternBaseUrl is required.");

        if (string.IsNullOrWhiteSpace(options.BodyId))
            throw new InvalidOperationException(
                "SmsProvider:BodyId is required.");
    }
}