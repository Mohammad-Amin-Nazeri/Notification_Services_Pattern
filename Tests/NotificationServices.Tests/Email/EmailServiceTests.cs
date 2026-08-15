using Moq;
using NotificationServices.Email;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.Abstractions.Models;

namespace NotificationServices.Tests.Email;

public sealed class EmailServiceTests
{
    [Fact]
    public async Task SendMessageAsync_WhenMessageIsNull_ShouldThrow()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.SendMessageAsync(null!));
    }

    [Fact]
    public async Task SendMessageAsync_WhenRecipientIsEmpty_ShouldThrow()
    {
        var service = CreateService();
        var message = new EmailMessage("", "Test", "Hello", false);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.SendMessageAsync(message));
    }

    [Fact]
    public async Task SendMessageAsync_WhenSubjectIsEmpty_ShouldThrow()
    {
        var service = CreateService();
        var message = new EmailMessage("test@example.com", "", "Hello", false);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.SendMessageAsync(message));
    }

    [Fact]
    public async Task SendMessageAsync_WhenBodyIsEmpty_ShouldThrow()
    {
        var service = CreateService();
        var message = new EmailMessage("test@example.com", "Test", "", false);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.SendMessageAsync(message));
    }

    [Fact]
    public async Task SendOtpAsync_WhenOtpIsNull_ShouldThrow()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.SendOtpAsync(null!));
    }

    [Fact]
    public async Task SendOtpAsync_WhenRecipientIsEmpty_ShouldThrow()
    {
        var service = CreateService();
        var otp = new EmailOtp("", "123456");

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.SendOtpAsync(otp));
    }

    [Fact]
    public async Task SendOtpAsync_WhenCodeIsEmpty_ShouldThrow()
    {
        var service = CreateService();
        var otp = new EmailOtp("test@example.com", "");

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.SendOtpAsync(otp));
    }

    [Fact]
    public async Task SendMessageAsync_WhenProviderConfigurationFails_ShouldPropagateException()
    {
        var optionsProvider = new Mock<IEmailProviderOptionsProvider>();
        optionsProvider
            .Setup(x => x.GetSettingAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Configuration is invalid."));

        var service = CreateService(optionsProvider.Object);
        var message = new EmailMessage("test@example.com", "Test", "Hello", false);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.SendMessageAsync(message));
    }

    [Fact]
    public async Task SendOtpAsync_WhenProviderConfigurationFails_ShouldPropagateException()
    {
        var optionsProvider = new Mock<IEmailProviderOptionsProvider>();
        optionsProvider
            .Setup(x => x.GetSettingAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Configuration is invalid."));

        var service = CreateService(optionsProvider.Object);
        var otp = new EmailOtp("test@example.com", "123456");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.SendOtpAsync(otp));
    }

    private static EmailService CreateService(
        IEmailProviderOptionsProvider? optionsProvider = null,
        IEmailTemplateProvider? templateProvider = null)
    {
        var options = new Mock<IEmailProviderOptionsProvider>();
        options.Setup(x => x.GetSettingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailProviderOptions
            {
                Host = "smtp.example.com",
                Port = 587,
                FromAddress = "noreply@example.com"
            });

        var templates = templateProvider ?? new Mock<IEmailTemplateProvider>().Object;
        return new EmailService(optionsProvider ?? options.Object, templates);
    }
}
