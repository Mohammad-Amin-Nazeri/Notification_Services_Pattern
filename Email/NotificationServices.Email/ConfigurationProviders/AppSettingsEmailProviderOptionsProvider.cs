using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.Abstractions.Models;
using Microsoft.Extensions.Configuration;

namespace NotificationServices.Email.ConfigurationProviders;

/// <summary>
/// Default <see cref="IEmailProviderOptionsProvider"/>: reads <see cref="EmailProviderOptions"/>
/// from the "EmailProvider" section of the app configuration.
///
/// To read the SMTP settings from a database instead, implement
/// <see cref="IEmailProviderOptionsProvider"/> yourself (e.g. against an EF Core DbContext)
/// and register it via <c>AddEmailService&lt;TOptionsProvider&gt;()</c> - <c>EmailService</c>
/// does not need any change.
/// </summary>
/// <example>
/// appsettings.json:
/// <code>
/// {
///   "EmailProvider": {
///     "Host": "smtp.example.com",
///     "Port": 587,
///     "EnableSsl": true,
///     "Username": "...",
///     "Password": "...",
///     "FromAddress": "no-reply@example.com",
///     "FromName": "My App"
///   }
/// }
/// </code>
/// </example>
public class AppSettingsEmailProviderOptionsProvider(IConfiguration configuration) : IEmailProviderOptionsProvider
{
    private const string SectionName = "EmailProvider";

    public Task<EmailProviderOptions> GetSettingAsync()
    {
        var section = configuration.GetSection(SectionName);
        if (!section.Exists())
        {
            throw new InvalidOperationException(
                $"Configuration section \"{SectionName}\" was not found. " +
                "Add it to appsettings.json, or register a different IEmailProviderOptionsProvider " +
                "(e.g. a database-backed one) via AddEmailService<TOptionsProvider>().");
        }

        var options = section.Get<EmailProviderOptions>()
                      ?? throw new InvalidOperationException($"Could not bind configuration section \"{SectionName}\".");

        return Task.FromResult(options);
    }
}
