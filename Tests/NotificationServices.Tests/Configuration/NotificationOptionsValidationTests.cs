using Microsoft.Extensions.Configuration;
using NotificationServices.Configuration;

namespace NotificationServices.Tests.Configuration;

public sealed class NotificationOptionsValidationTests
{
    [Fact]
    public async Task AppSettingsProvider_WithValidConfiguration_ReturnsOptions()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["NotificationServices:Email:Host"] = "smtp.example.com",
                ["NotificationServices:Email:Port"] = "587",
                ["NotificationServices:Email:EnableSsl"] = "true",
                ["NotificationServices:Email:FromAddress"] = "noreply@example.com",
                ["NotificationServices:Sms:ProviderType"] = "Melipayamak",
                ["NotificationServices:Sms:Username"] = "user",
                ["NotificationServices:Sms:Password"] = "secret",
                ["NotificationServices:Sms:From"] = "50004001",
                ["NotificationServices:Sms:BaseUrl"] = "https://example.com/send",
                ["NotificationServices:Sms:PatternBaseUrl"] = "https://example.com/pattern"
            })
            .Build();

        var provider = new AppSettingsNotificationOptionsProvider(configuration);
        var options = await provider.GetOptionsAsync();

        Assert.Equal("smtp.example.com", options.Email.Host);
        Assert.Equal(587, options.Email.Port);
        Assert.Equal("Melipayamak", options.Sms.ProviderType);
    }

    [Fact]
    public async Task AppSettingsProvider_WithMissingRequiredConfiguration_ThrowsHelpfulException()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var provider = new AppSettingsNotificationOptionsProvider(configuration);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.GetOptionsAsync().AsTask());

        Assert.Contains("NotificationServices:Email:Host is required.", exception.Message);
        Assert.Contains("NotificationServices:Sms:ProviderType is required.", exception.Message);
        Assert.Contains("NotificationServices:Sms:BaseUrl is required.", exception.Message);
    }

    [Fact]
    public async Task AppSettingsProvider_WithInvalidValues_ThrowsHelpfulException()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["NotificationServices:Email:Host"] = "smtp.example.com",
                ["NotificationServices:Email:Port"] = "70000",
                ["NotificationServices:Email:FromAddress"] = "not-an-email",
                ["NotificationServices:Sms:ProviderType"] = "Melipayamak",
                ["NotificationServices:Sms:Username"] = "user",
                ["NotificationServices:Sms:Password"] = "secret",
                ["NotificationServices:Sms:From"] = "50004001",
                ["NotificationServices:Sms:BaseUrl"] = "not-a-uri"
            })
            .Build();

        var provider = new AppSettingsNotificationOptionsProvider(configuration);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.GetOptionsAsync().AsTask());

        Assert.Contains("NotificationServices:Email:Port must be between 1 and 65535.", exception.Message);
        Assert.Contains("NotificationServices:Email:FromAddress must be a valid email address.", exception.Message);
        Assert.Contains("NotificationServices:Sms:BaseUrl must be a valid absolute URI.", exception.Message);
    }
}
