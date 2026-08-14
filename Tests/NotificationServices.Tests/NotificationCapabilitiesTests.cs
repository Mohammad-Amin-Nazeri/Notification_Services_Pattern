using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Configuration;
using NotificationServices.DependencyInjection;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.Abstractions.Models;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Tests;

public sealed class NotificationCapabilitiesTests
{
    [Fact]
    public async Task DisabledCapabilities_PreventEmailAndSmsSending()
    {
        var services = new ServiceCollection();
        services.AddNotificationServices();
        services.AddNotificationCapabilitiesProvider<DisabledCapabilitiesProvider>();

        await using var provider = services.BuildServiceProvider();
        var email = provider.GetRequiredService<IEmailService>();
        var sms = provider.GetRequiredService<ISmsService>();

        var emailResult = await email.SendMessageAsync(new EmailMessage
        {
            To = "test@example.com",
            Subject = "test",
            Body = "test"
        });

        var smsResult = await sms.SendMessageAsync(new SmsMessage
        {
            Mobile = "09120000000",
            Text = "test"
        });

        Assert.False(emailResult.IsSuccess);
        Assert.Equal("email.disabled", emailResult.ErrorCode);
        Assert.False(smsResult.IsSuccess);
        Assert.Equal("sms.disabled", smsResult.ErrorCode);
    }

    private sealed class DisabledCapabilitiesProvider : INotificationCapabilitiesProvider
    {
        public ValueTask<NotificationCapabilities> GetCapabilitiesAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(new NotificationCapabilities
            {
                EmailEnabled = false,
                SmsEnabled = false
            });
        }
    }
}
