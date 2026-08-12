using NotificationServices.Configuration;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.Abstractions.Models;

namespace NotificationServices.DependencyInjection;

internal sealed class EmailOptionsAdapter(
    INotificationOptionsProvider optionsProvider) : IEmailProviderOptionsProvider
{
    public async Task<EmailProviderOptions> GetSettingAsync(
        CancellationToken cancellationToken = default)
    {
        var options = await optionsProvider.GetOptionsAsync(cancellationToken);

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
