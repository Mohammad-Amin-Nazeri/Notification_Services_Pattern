using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.Abstractions.Models;
using NotificationServices.Email.DependencyInjection;

namespace NotificationServices.Tests.Email;

public sealed class EmailTemplateProviderTests
{
    [Fact]
    public async Task DefaultTemplateProvider_ReturnsOtpTemplate()
    {
        var services = new ServiceCollection();
        services.AddEmailService();

        await using var provider = services.BuildServiceProvider();
        var templateProvider = provider.GetRequiredService<IEmailTemplateProvider>();

        var template = await templateProvider.GetOtpTemplateAsync(
            new EmailOtp("user@example.com", "123456"));

        Assert.Equal("Verification Code", template.Subject);
        Assert.Contains("123456", template.Body);
        Assert.True(template.IsHtml);
    }

    [Fact]
    public async Task CustomTemplateProvider_ReplacesDefaultProvider()
    {
        var services = new ServiceCollection();
        services.AddEmailService();
        services.AddEmailTemplateProvider<CustomTemplateProvider>();

        await using var provider = services.BuildServiceProvider();
        var templateProvider = provider.GetRequiredService<IEmailTemplateProvider>();

        var template = await templateProvider.GetOtpTemplateAsync(
            new EmailOtp("user@example.com", "123456"));

        Assert.Equal("Custom OTP", template.Subject);
        Assert.Equal("OTP: 123456", template.Body);
        Assert.False(template.IsHtml);
    }

    private sealed class CustomTemplateProvider : IEmailTemplateProvider
    {
        public ValueTask<EmailTemplate> GetOtpTemplateAsync(
            EmailOtp otp,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(new EmailTemplate(
                "Custom OTP",
                $"OTP: {otp.Code}",
                IsHtml: false));
        }
    }
}
