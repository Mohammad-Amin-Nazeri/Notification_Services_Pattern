using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.Abstractions.Models;
using NotificationServices.Options;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Configuration;

internal sealed class EmailOptionsAdapter : IEmailProviderOptionsProvider
{
    private readonly INotificationOptionsProvider _provider;

    public EmailOptionsAdapter(INotificationOptionsProvider provider) => _provider = provider;

    public async Task<EmailProviderOptions> GetSettingAsync(CancellationToken cancellationToken = default)
    {
        var options = await _provider.GetOptionsAsync(cancellationToken);
        return new EmailProviderOptions
        {
            Host = options.Email.Host,
            Port = options.Email.Port,
            EnableSsl = options.Email.EnableSsl,
            Username = options.Email.Username,
            Password = options.Email.Password,
            FromAddress = options.Email.FromAddress,
            FromName = options.Email.FromName
        };
    }
}

internal sealed class SmsOptionsAdapter : ISmsProviderOptionsProvider
{
    private readonly INotificationOptionsProvider _provider;

    public SmsOptionsAdapter(INotificationOptionsProvider provider) => _provider = provider;

    public async Task<SmsProviderOptions> GetSettingAsync(CancellationToken cancellationToken = default)
    {
        var options = await _provider.GetOptionsAsync(cancellationToken);
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
