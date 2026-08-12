using Microsoft.Extensions.DependencyInjection;
using Moq;
using NotificationServices.Sms;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Tests.Sms;

public sealed class SmsProviderFactoryTests
{
    [Fact]
    public async Task GetProviderAsync_WhenProviderTypeIsUnknown_ShouldThrow()
    {
        var optionsProvider =
            new Mock<ISmsProviderOptionsProvider>();

        optionsProvider
            .Setup(x => x.GetSettingAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new SmsProviderOptions
                {
                    ProviderType = "UnknownProvider"
                });

        var services = new ServiceCollection();

        services.AddHttpClient();

        await using var serviceProvider =
            services.BuildServiceProvider();

        var httpClientFactory =
            serviceProvider.GetRequiredService<IHttpClientFactory>();

        var factory = new SmsProviderFactory(
            optionsProvider.Object,
            httpClientFactory);

        await Assert.ThrowsAsync<NotSupportedException>(
            () => factory.GetProviderAsync());
    }

    [Fact]
    public async Task GetProviderAsync_WhenProviderTypeIsEmpty_ShouldThrow()
    {
        var optionsProvider =
            new Mock<ISmsProviderOptionsProvider>();

        optionsProvider
            .Setup(x => x.GetSettingAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new SmsProviderOptions
                {
                    ProviderType = ""
                });

        var services = new ServiceCollection();

        services.AddHttpClient();

        await using var serviceProvider =
            services.BuildServiceProvider();

        var httpClientFactory =
            serviceProvider.GetRequiredService<IHttpClientFactory>();

        var factory = new SmsProviderFactory(
            optionsProvider.Object,
            httpClientFactory);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => factory.GetProviderAsync());
    }

    [Fact]
    public async Task GetProviderAsync_WhenProviderIsMelipayamak_ShouldReturnProvider()
    {
        var optionsProvider =
            new Mock<ISmsProviderOptionsProvider>();

        optionsProvider
            .Setup(x => x.GetSettingAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new SmsProviderOptions
                {
                    ProviderType = "Melipayamak",
                    Username = "username",
                    Password = "password",
                    From = "50004001",
                    BaseUrl = "https://example.com/send",
                    PatternBaseUrl = "https://example.com/pattern",
                    BodyId = "12345"
                });

        var services = new ServiceCollection();

        services.AddHttpClient();

        await using var serviceProvider =
            services.BuildServiceProvider();

        var httpClientFactory =
            serviceProvider.GetRequiredService<IHttpClientFactory>();

        var factory = new SmsProviderFactory(
            optionsProvider.Object,
            httpClientFactory);

        var provider =
            await factory.GetProviderAsync();

        Assert.NotNull(provider);
        Assert.IsAssignableFrom<ISmsProvider>(
            provider);
    }
}