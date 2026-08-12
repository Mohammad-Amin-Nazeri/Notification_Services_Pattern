using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.Abstractions.Interfaces;

/// <summary>
/// Reads <see cref="SmsProviderOptions"/> from wherever they are stored.
///
/// This is the ONE seam to touch if you want to change the settings source later
/// (e.g. move from appsettings.json to a database): implement this interface against
/// your own storage (an EF Core DbContext, for example) and register it in DI instead
/// of the default <c>AppSettingsSmsProviderOptionsProvider</c>. Nothing else in this
/// library needs to change.
/// </summary>
public interface ISmsProviderOptionsProvider
{
    Task<SmsProviderOptions> GetSettingAsync();
}
