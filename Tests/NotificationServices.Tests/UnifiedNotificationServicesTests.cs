using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Configuration;
using NotificationServices.DependencyInjection;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Options;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;
using NotificationServices.Sms.DependencyInjection;

namespace NotificationServices.Tests;

public sealed class UnifiedNotificationServicesTests
{
    [Fact]
    public void AddNotificationServices_UsesAppSettingsProviderByDefault()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["NotificationServices:Email:Host"] = "smtp.example.com",
                ["NotificationServices:Email:Port"] = "587",
                ["NotificationServices:Email:FromAddress"] = "noreply@example.com",
                ["NotificationServices:Sms:ProviderType"] = "Melipayamak",
                ["NotificationServices:Sms:Username"] = "user",
                ["NotificationServices:Sms:Password"] = "password",
                ["NotificationServices:Sms:From"] = "50004001",
                ["NotificationServices:Sms:BaseUrl"] = "https://example.com/send"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddNotificationServices();

        using var provider = services.BuildServiceProvider();

        Assert.IsType<AppSettingsNotificationOptionsProvider>(
            provider.GetRequiredService<INotificationOptionsProvider>());
        Assert.NotNull(provider.GetRequiredService<IEmailService>());
        Assert.NotNull(provider.GetRequiredService<ISmsService>());
        Assert.NotNull(provider.GetRequiredService<ISmsProviderFactory>());
    }

    [Fact]
    public async Task AddNotificationServices_WithCustomProvider_UsesApplicationProvider()
    {
        var services = new ServiceCollection();
        services.AddNotificationServices<TestNotificationOptionsProvider>();

        using var provider = services.BuildServiceProvider();
        var optionsProvider = provider.GetRequiredService<INotificationOptionsProvider>();
        var options = await optionsProvider.GetOptionsAsync();

        Assert.Equal("custom.smtp.local", options.Email.Host);
        Assert.Equal("Custom", options.Sms.ProviderType);
    }

    private sealed class TestNotificationOptionsProvider : INotificationOptionsProvider
    {
        public ValueTask<NotificationOptions> GetOptionsAsync(
            CancellationToken cancellationToken = default)
            => ValueTask.FromResult(new NotificationOptions
            {
                Email = new() { Host = "custom.smtp.local" },
                Sms = new() { ProviderType = "Custom" }
            });
    }
}
