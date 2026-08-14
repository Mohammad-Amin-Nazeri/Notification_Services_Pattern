using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;
using NotificationServices.Sms.DependencyInjection;

namespace NotificationServices.Tests.Sms;

public sealed class SmsProviderRegistryTests
{
    [Fact]
    public async Task Resolves_provider_selected_by_runtime_options()
    {
        var services = new ServiceCollection();
        services.AddSmsService<FixedOptionsProvider>();
        services.AddSmsProvider<FakeSmsProvider>("Fake");

        await using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ISmsProviderFactory>();

        var resolved = await factory.GetProviderAsync();

        Assert.IsType<FakeSmsProvider>(resolved);
    }

    [Fact]
    public async Task Unknown_provider_fails_without_changing_registry()
    {
        var services = new ServiceCollection();
        services.AddSmsService<UnknownOptionsProvider>();
        services.AddSmsProvider<FakeSmsProvider>("Fake");

        await using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ISmsProviderFactory>();

        var exception = await Assert.ThrowsAsync<NotSupportedException>(
            () => factory.GetProviderAsync());

        Assert.Contains("Missing", exception.Message);
    }

    private sealed class FixedOptionsProvider : ISmsProviderOptionsProvider
    {
        public Task<SmsProviderOptions> GetSettingAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new SmsProviderOptions
            {
                ProviderType = "Fake"
            });
    }

    private sealed class UnknownOptionsProvider : ISmsProviderOptionsProvider
    {
        public Task<SmsProviderOptions> GetSettingAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new SmsProviderOptions
            {
                ProviderType = "Missing"
            });
    }

    private sealed class FakeSmsProvider(SmsProviderOptions options) : ISmsProvider
    {
        public Task<SmsResult> SendMessageAsync(
            SmsMessage message,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(SmsResult.Success("fake"));

        public Task<SmsResult> SendOtpAsync(
            SmsOtp otp,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(SmsResult.Success("fake"));
    }
}
