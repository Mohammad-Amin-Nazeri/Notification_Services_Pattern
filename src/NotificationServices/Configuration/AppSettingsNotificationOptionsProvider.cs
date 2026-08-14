using Microsoft.Extensions.Configuration;
using NotificationServices.Options;

namespace NotificationServices.Configuration;

public sealed class AppSettingsNotificationOptionsProvider : INotificationOptionsProvider
{
    private readonly IConfiguration _configuration;

    public AppSettingsNotificationOptionsProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ValueTask<NotificationOptions> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var options = new NotificationOptions
        {
            Email = _configuration.GetSection("NotificationServices:Email").Get<EmailOptions>() ?? new(),
            Sms = _configuration.GetSection("NotificationServices:Sms").Get<SmsOptions>() ?? new()
        };

        NotificationOptionsValidator.Validate(options);
        return ValueTask.FromResult(options);
    }
}
