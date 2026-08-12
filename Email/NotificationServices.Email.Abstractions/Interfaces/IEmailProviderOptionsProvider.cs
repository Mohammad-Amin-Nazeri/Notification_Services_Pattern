using NotificationServices.Email.Abstractions.Models;

namespace NotificationServices.Email.Abstractions.Interfaces;

/// <summary>
/// Provides email provider configuration.
/// The configuration source can be appsettings, database, environment variables,
/// or any other custom source.
/// </summary>
public interface IEmailProviderOptionsProvider
{
    Task<EmailProviderOptions> GetSettingAsync(
        CancellationToken cancellationToken = default);
}