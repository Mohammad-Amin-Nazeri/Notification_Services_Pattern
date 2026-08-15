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
        var optionsProvider = new Mock<ISmsProviderOptionsProvider>();
        var registry = new Mock<ISmsProviderRegistry>();

        optionsProvider
            .Setup(x => x.GetSettingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SmsProviderOptions
            {
                ProviderType = "UnknownProvider"
            });

        registry
            .Setup(x => x.CreateProvider(
                "UnknownProvider",
                It.IsAny<SmsProviderOptions>()))
            .Throws(new NotSupportedException("SMS provider 'UnknownProvider' is not registered."));

        var factory = new SmsProviderFactory(
            optionsProvider.Object,
            registry.Object);

        await Assert.ThrowsAsync<NotSupportedException>(
            () => factory.GetProviderAsync());
    }

    [Fact]
    public async Task GetProviderAsync_WhenProviderTypeIsEmpty_ShouldThrow()
    {
        var optionsProvider = new Mock<ISmsProviderOptionsProvider>();
        var registry = new Mock<ISmsProviderRegistry>();

        optionsProvider
            .Setup(x => x.GetSettingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SmsProviderOptions
            {
                ProviderType = string.Empty
            });

        var factory = new SmsProviderFactory(
            optionsProvider.Object,
            registry.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => factory.GetProviderAsync());

        registry.Verify(
            x => x.CreateProvider(
                It.IsAny<string>(),
                It.IsAny<SmsProviderOptions>()),
            Times.Never);
    }

    [Fact]
    public async Task GetProviderAsync_WhenProviderIsMelipayamak_ShouldReturnProvider()
    {
        var optionsProvider = new Mock<ISmsProviderOptionsProvider>();
        var registry = new Mock<ISmsProviderRegistry>();
        var provider = new Mock<ISmsProvider>().Object;

        var options = new SmsProviderOptions
        {
            ProviderType = "Melipayamak",
            Username = "username",
            Password = "password",
            From = "50004001",
            BaseUrl = "https://example.com/send",
            PatternBaseUrl = "https://example.com/pattern",
            ProviderSettings = new Dictionary<string, string>
            {
                ["BodyId"] = "12345"
            }
        };

        optionsProvider
            .Setup(x => x.GetSettingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(options);

        registry
            .Setup(x => x.CreateProvider(
                "Melipayamak",
                options))
            .Returns(provider);

        var factory = new SmsProviderFactory(
            optionsProvider.Object,
            registry.Object);

        var resolved = await factory.GetProviderAsync();

        Assert.Same(provider, resolved);
        registry.Verify(
            x => x.CreateProvider("Melipayamak", options),
            Times.Once);
    }
}
