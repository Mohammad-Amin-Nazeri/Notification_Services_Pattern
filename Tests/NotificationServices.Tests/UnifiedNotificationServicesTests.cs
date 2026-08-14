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
                ["NotificationServices:Sms:ProviderType"] = "Example"
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

    [Fact]
    public async Task AddNotificationServices_WithScopedOptionsProvider_SelectsProviderPerLicense()
    {
        var services = new ServiceCollection();
        services.AddScoped<TestLicenseContext>();
        services.AddNotificationServices<TestLicenseAwareOptionsProvider>();
        services.AddSmsProvider<TestKavenegarSmsProvider>("Kavenegar");

        await using var provider = services.BuildServiceProvider();

        await using (var licenseScope = provider.CreateAsyncScope())
        {
            var context = licenseScope.ServiceProvider.GetRequiredService<TestLicenseContext>();
            context.LicenseId = "license-melipayamak";

            var factory = licenseScope.ServiceProvider.GetRequiredService<ISmsProviderFactory>();
            var resolved = await factory.GetProviderAsync();

            Assert.IsType<NotificationServices.Sms.Providers.MelipayamakSmsProvider>(resolved);
        }

        await using (var licenseScope = provider.CreateAsyncScope())
        {
            var context = licenseScope.ServiceProvider.GetRequiredService<TestLicenseContext>();
            context.LicenseId = "license-kavenegar";

            var factory = licenseScope.ServiceProvider.GetRequiredService<ISmsProviderFactory>();
            var resolved = await factory.GetProviderAsync();

            Assert.IsType<TestKavenegarSmsProvider>(resolved);
        }
    }

    [Fact]
    public async Task AddNotificationServices_WithScopedOptionsProvider_DoesNotLeakProviderBetweenLicenses()
    {
        var services = new ServiceCollection();
        services.AddScoped<TestLicenseContext>();
        services.AddNotificationServices<TestLicenseAwareOptionsProvider>();
        services.AddSmsProvider<TestKavenegarSmsProvider>("Kavenegar");

        await using var provider = services.BuildServiceProvider();

        await using var melipayamakScope = provider.CreateAsyncScope();
        await using var kavenegarScope = provider.CreateAsyncScope();

        melipayamakScope.ServiceProvider
            .GetRequiredService<TestLicenseContext>()
            .LicenseId = "license-melipayamak";

        kavenegarScope.ServiceProvider
            .GetRequiredService<TestLicenseContext>()
            .LicenseId = "license-kavenegar";

        var melipayamakFactory = melipayamakScope.ServiceProvider
            .GetRequiredService<ISmsProviderFactory>();
        var kavenegarFactory = kavenegarScope.ServiceProvider
            .GetRequiredService<ISmsProviderFactory>();

        var melipayamakProvider = await melipayamakFactory.GetProviderAsync();
        var kavenegarProvider = await kavenegarFactory.GetProviderAsync();

        Assert.IsType<NotificationServices.Sms.Providers.MelipayamakSmsProvider>(melipayamakProvider);
        Assert.IsType<TestKavenegarSmsProvider>(kavenegarProvider);
    }

    private sealed class TestNotificationOptionsProvider : INotificationOptionsProvider
    {
        public ValueTask<NotificationOptions> GetOptionsAsync(CancellationToken cancellationToken = default)
            => ValueTask.FromResult(new NotificationOptions
            {
                Email = new() { Host = "custom.smtp.local" },
                Sms = new() { ProviderType = "Custom" }
            });
    }

    private sealed class TestLicenseContext
    {
        public string LicenseId { get; set; } = string.Empty;
    }

    private sealed class TestLicenseAwareOptionsProvider(
        TestLicenseContext context) : INotificationOptionsProvider
    {
        public ValueTask<NotificationOptions> GetOptionsAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var provider = context.LicenseId switch
            {
                "license-melipayamak" => "Melipayamak",
                "license-kavenegar" => "Kavenegar",
                _ => throw new InvalidOperationException(
                    $"License '{context.LicenseId}' is not configured.")
            };

            return ValueTask.FromResult(new NotificationOptions
            {
                Email = new() { Host = "smtp.example.com", Port = 587 },
                Sms = new()
                {
                    ProviderType = provider,
                    Username = "test-user",
                    Password = "test-password",
                    From = "50004001",
                    BaseUrl = "https://example.com/send",
                    PatternBaseUrl = "https://example.com/pattern",
                    BodyId = "12345"
                }
            });
        }
    }

    private sealed class TestKavenegarSmsProvider(SmsProviderOptions options) : ISmsProvider
    {
        public SmsProviderOptions Options { get; } = options;

        public Task<SmsResult> SendMessageAsync(
            SmsMessage message,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(SmsResult.Success("kavenegar-test"));

        public Task<SmsResult> SendOtpAsync(
            SmsOtp otp,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(SmsResult.Success("kavenegar-test"));
    }
}
