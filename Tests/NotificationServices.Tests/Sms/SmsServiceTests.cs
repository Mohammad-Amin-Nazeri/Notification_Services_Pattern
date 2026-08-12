using Moq;
using NotificationServices.Sms;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Tests.Sms;

public sealed class SmsServiceTests
{
    [Fact]
    public async Task SendMessageAsync_WhenMessageIsNull_ShouldThrow()
    {
        var factory = new Mock<ISmsProviderFactory>();

        var service = new SmsService(
            factory.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.SendMessageAsync(null!));
    }

    [Fact]
    public async Task SendMessageAsync_WhenMobileIsEmpty_ShouldThrow()
    {
        var factory = new Mock<ISmsProviderFactory>();

        var service = new SmsService(
            factory.Object);

        var message = new SmsMessage(
            "",
            "Hello");

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.SendMessageAsync(message));
    }

    [Fact]
    public async Task SendMessageAsync_WhenTextIsEmpty_ShouldThrow()
    {
        var factory = new Mock<ISmsProviderFactory>();

        var service = new SmsService(
            factory.Object);

        var message = new SmsMessage(
            "09120000000",
            "");

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.SendMessageAsync(message));
    }

    [Fact]
    public async Task SendOtpAsync_WhenOtpIsNull_ShouldThrow()
    {
        var factory = new Mock<ISmsProviderFactory>();

        var service = new SmsService(
            factory.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.SendOtpAsync(null!));
    }

    [Fact]
    public async Task SendOtpAsync_WhenMobileIsEmpty_ShouldThrow()
    {
        var factory = new Mock<ISmsProviderFactory>();

        var service = new SmsService(
            factory.Object);

        var otp = new SmsOtp(
            "",
            "123456");

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.SendOtpAsync(otp));
    }

    [Fact]
    public async Task SendOtpAsync_WhenCodeIsEmpty_ShouldThrow()
    {
        var factory = new Mock<ISmsProviderFactory>();

        var service = new SmsService(
            factory.Object);

        var otp = new SmsOtp(
            "09120000000",
            "");

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.SendOtpAsync(otp));
    }

    [Fact]
    public async Task SendMessageAsync_ShouldUseProvider()
    {
        var factory = new Mock<ISmsProviderFactory>();
        var provider = new Mock<ISmsProvider>();

        var expectedResult = SmsResult.Success(
            "Message sent successfully.");

        var message = new SmsMessage(
            "09120000000",
            "Hello");

        factory
            .Setup(x => x.GetProviderAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(provider.Object);

        provider
            .Setup(x => x.SendMessageAsync(
                message,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var service = new SmsService(
            factory.Object);

        var result = await service.SendMessageAsync(
            message);

        Assert.True(result.IsSuccess);
        Assert.Equal(
            expectedResult.Message,
            result.Message);

        provider.Verify(
            x => x.SendMessageAsync(
                message,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendOtpAsync_ShouldUseProvider()
    {
        var factory = new Mock<ISmsProviderFactory>();
        var provider = new Mock<ISmsProvider>();

        var expectedResult = SmsResult.Success(
            "OTP sent successfully.");

        var otp = new SmsOtp(
            "09120000000",
            "123456");

        factory
            .Setup(x => x.GetProviderAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(provider.Object);

        provider
            .Setup(x => x.SendOtpAsync(
                otp,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var service = new SmsService(
            factory.Object);

        var result = await service.SendOtpAsync(
            otp);

        Assert.True(result.IsSuccess);
        Assert.Equal(
            expectedResult.Message,
            result.Message);

        provider.Verify(
            x => x.SendOtpAsync(
                otp,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}




