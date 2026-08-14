using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Abstractions;
using NotificationServices.DependencyInjection;

namespace NotificationServices.Tests;

public sealed class NotificationCapabilitiesTests
{
    [Fact]
    public async Task AddNotificationServices_RegistersDefaultCapabilitiesProvider()
    {
        var services = new ServiceCollection();
        services.AddNotificationServices();

        await using var provider = services.BuildServiceProvider();
        var capabilitiesProvider = provider.GetRequiredService<INotificationCapabilitiesProvider>();

        var capabilities = await capabilitiesProvider.GetCapabilitiesAsync();

        Assert.True(capabilities.EmailEnabled);
        Assert.True(capabilities.SmsEnabled);
    }

    [Fact]
    public async Task CustomCapabilitiesProvider_OverridesDefaultProvider()
    {
        var services = new ServiceCollection();
        services.AddNotificationServices();
        services.AddNotificationCapabilitiesProvider<DisabledCapabilitiesProvider>();

        await using var provider = services.BuildServiceProvider();
        var capabilitiesProvider = provider.GetRequiredService<INotificationCapabilitiesProvider>();

        var capabilities = await capabilitiesProvider.GetCapabilitiesAsync();

        Assert.False(capabilities.EmailEnabled);
        Assert.False(capabilities.SmsEnabled);
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
