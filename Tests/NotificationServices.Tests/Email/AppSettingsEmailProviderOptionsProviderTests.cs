using Microsoft.Extensions.Configuration;
using NotificationServices.Email.ConfigurationProviders;

namespace NotificationServices.Tests.Email;

public sealed class AppSettingsEmailProviderOptionsProviderTests
{
    [Fact]
    public async Task GetSettingAsync_WhenConfigurationIsValid_ShouldReturnOptions()
    {
        var configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["EmailProvider:Host"] = "smtp.example.com",
                        ["EmailProvider:Port"] = "587",
                        ["EmailProvider:EnableSsl"] = "true",
                        ["EmailProvider:Username"] = "user",
                        ["EmailProvider:Password"] = "password",
                        ["EmailProvider:FromAddress"] = "no-reply@example.com",
                        ["EmailProvider:FromName"] = "Notification Service"
                    })
                .Build();

        var provider =
            new AppSettingsEmailProviderOptionsProvider(
                configuration);

        var result =
            await provider.GetSettingAsync();

        Assert.Equal(
            "smtp.example.com",
            result.Host);

        Assert.Equal(
            587,
            result.Port);

        Assert.True(
            result.EnableSsl);

        Assert.Equal(
            "no-reply@example.com",
            result.FromAddress);
    }

    [Fact]
    public async Task GetSettingAsync_WhenSectionDoesNotExist_ShouldThrow()
    {
        var configuration =
            new ConfigurationBuilder()
                .Build();

        var provider =
            new AppSettingsEmailProviderOptionsProvider(
                configuration);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.GetSettingAsync());
    }

    [Fact]
    public async Task GetSettingAsync_WhenHostIsMissing_ShouldThrow()
    {
        var configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["EmailProvider:Port"] = "587",
                        ["EmailProvider:EnableSsl"] = "true",
                        ["EmailProvider:FromAddress"] =
                            "no-reply@example.com"
                    })
                .Build();

        var provider =
            new AppSettingsEmailProviderOptionsProvider(
                configuration);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.GetSettingAsync());
    }

    [Fact]
    public async Task GetSettingAsync_WhenPortIsInvalid_ShouldThrow()
    {
        var configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["EmailProvider:Host"] =
                            "smtp.example.com",

                        ["EmailProvider:Port"] =
                            "0",

                        ["EmailProvider:FromAddress"] =
                            "no-reply@example.com"
                    })
                .Build();

        var provider =
            new AppSettingsEmailProviderOptionsProvider(
                configuration);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.GetSettingAsync());
    }
}