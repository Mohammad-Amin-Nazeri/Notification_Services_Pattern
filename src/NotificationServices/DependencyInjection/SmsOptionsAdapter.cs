using NotificationServices.Configuration;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.DependencyInjection;

internal sealed class SmsOptionsAdapter(
    INotificationOptionsProvider optionsProvider) : ISmsProviderOptionsProvider
{
    public async Task<SmsProviderOptions> GetSettingAsync(
        CancellationToken cancellationToken = default)
    {
        var options = await optionsProvider.GetOptionsAsync(cancellationToken);

        return new SmsProviderOptions
        {
            ProviderType = options.Sms.ProviderType,
            Username = options.Sms.Username,
            Password = options.Sms.Password,
            From = options.Sms.From,
            BaseUrl = options.Sms.BaseUrl,
            PatternBaseUrl = options.Sms.PatternBaseUrl,
            BodyId = options.Sms.BodyId
        };
    }
}
