using NotificationServices.Email.Abstractions.Models;

namespace NotificationServices.Email.Abstractions.Interfaces;

/// <summary>
/// Reads <see cref="EmailProviderOptions"/> from wherever they are stored.
///
/// Same idea as ISmsProviderOptionsProvider: this is the ONE seam to change if the settings
/// source moves from appsettings.json to a database - implement this interface against your
/// own storage and register it via AddEmailService&lt;TOptionsProvider&gt;(). EmailService
/// itself never needs to change.
/// </summary>
public interface IEmailProviderOptionsProvider
{
    Task<EmailProviderOptions> GetSettingAsync();
}
