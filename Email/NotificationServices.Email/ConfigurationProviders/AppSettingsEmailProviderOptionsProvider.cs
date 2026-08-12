using Microsoft.Extensions.Configuration;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.Abstractions.Models;

namespace NotificationServices.Email.ConfigurationProviders;

public sealed class AppSettingsEmailProviderOptionsProvider(
    IConfiguration configuration) : IEmailProviderOptionsProvider
{
    private const string SectionName = "EmailProvider";

    public Task<EmailProviderOptions> GetSettingAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var section = configuration.GetSection(SectionName);

        if (!section.Exists())
        {
            throw new InvalidOperationException(
                $"Configuration section '{SectionName}' was not found.");
        }

        var options = section.Get<EmailProviderOptions>();

        if (options is null)
        {
            throw new InvalidOperationException(
                $"Could not bind configuration section '{SectionName}'.");
        }

        Validate(options);

        return Task.FromResult(options);
    }

    private static void Validate(EmailProviderOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Host))
            throw new InvalidOperationException(
                "EmailProvider:Host is required.");

        if (options.Port <= 0)
            throw new InvalidOperationException(
                "EmailProvider:Port must be greater than zero.");

        if (string.IsNullOrWhiteSpace(options.FromAddress))
            throw new InvalidOperationException(
                "EmailProvider:FromAddress is required.");
    }
}