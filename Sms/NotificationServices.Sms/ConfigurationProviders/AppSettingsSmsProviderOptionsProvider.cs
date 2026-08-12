using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;
using Microsoft.Extensions.Configuration;

namespace NotificationServices.Sms.ConfigurationProviders;

/// <summary>
/// Default <see cref="ISmsProviderOptionsProvider"/>: reads <see cref="SmsProviderOptions"/>
/// from the "SmsProvider" section of the app configuration (appsettings.json, environment
/// variables, Azure App Configuration, ...) via <see cref="IConfiguration"/>.
///
/// If you later need the settings to come from a database instead, write your own
/// implementation of <see cref="ISmsProviderOptionsProvider"/> (for example, one that reads
/// from an EF Core DbContext) and register it via the generic
/// <c>AddSmsService&lt;TOptionsProvider&gt;()</c> overload - that is the only change needed.
/// </summary>
/// <example>
/// appsettings.json:
/// <code>
/// {
///   "SmsProvider": {
///     "ProviderType": "Melipayamak",
///     "Username": "...",
///     "Password": "...",
///     "From": "50004001",
///     "BodyId": "...",
///     "Melipayamak": { "BaseUrl": "https://rest.payamak-panel.com/api/SendSMS/BaseServiceNumber" }
///   }
/// }
/// </code>
/// </example>
public class AppSettingsSmsProviderOptionsProvider(IConfiguration configuration) : ISmsProviderOptionsProvider
{
    private const string SectionName = "SmsProvider";

    public Task<SmsProviderOptions> GetSettingAsync()
    {
        var section = configuration.GetSection(SectionName);
        if (!section.Exists())
        {
            throw new InvalidOperationException(
                $"Configuration section \"{SectionName}\" was not found. " +
                "Add it to appsettings.json, or register a different ISmsProviderOptionsProvider " +
                "(e.g. a database-backed one) via AddSmsService<TOptionsProvider>().");
        }

        var options = section.Get<SmsProviderOptions>()
            ?? throw new InvalidOperationException($"Could not bind configuration section \"{SectionName}\".");

        // Kept async (Task.FromResult) on purpose: it preserves the exact same
        // ISmsProviderOptionsProvider contract used by database-backed providers,
        // so callers never need to know whether the source is sync or async.
        return Task.FromResult(options);
    }
}
