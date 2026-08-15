using Microsoft.Extensions.Configuration;
using NotificationServices.Sms.ConfigurationProviders;

namespace NotificationServices.Tests.Sms;

public sealed class AppSettingsSmsProviderOptionsProviderTests
{
    [Fact]
    public async Task GetSettingAsync_WhenConfigurationIsValid_ShouldReturnOptions()
    {
        var configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["SmsProvider:ProviderType"] =
                            "Melipayamak",

                        ["SmsProvider:Username"] =
                            "username",

                        ["SmsProvider:Password"] =
                            "password",

                        ["SmsProvider:From"] =
                            "50004001",

                        ["SmsProvider:BaseUrl"] =
                            "https://example.com/send",

                        ["SmsProvider:PatternBaseUrl"] =
                            "https://example.com/pattern",

                        ["SmsProvider:BodyId"] =
                            "12345"
                    })
                .Build();

        var provider =
            new AppSettingsSmsProviderOptionsProvider(
                configuration);

        var result =
            await provider.GetSettingAsync();

        Assert.Equal(
            "Melipayamak",
            result.ProviderType);

        Assert.Equal(
            "username",
            result.Username);

        Assert.Equal(
            "50004001",
            result.From);

        Assert.Equal(
            "12345",
            result.GetRequiredProviderSetting("BodyId"));
    }

    [Fact]
    public async Task GetSettingAsync_WhenSectionDoesNotExist_ShouldThrow()
    {
        var configuration =
            new ConfigurationBuilder()
                .Build();

        var provider =
            new AppSettingsSmsProviderOptionsProvider(
                configuration);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.GetSettingAsync());
    }

    [Fact]
    public async Task GetSettingAsync_WhenProviderTypeIsMissing_ShouldThrow()
    {
        var configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["SmsProvider:Username"] =
                            "username",

                        ["SmsProvider:Password"] =
                            "password",

                        ["SmsProvider:From"] =
                            "50004001",

                        ["SmsProvider:BaseUrl"] =
                            "https://example.com/send",

                        ["SmsProvider:PatternBaseUrl"] =
                            "https://example.com/pattern",

                        ["SmsProvider:BodyId"] =
                            "12345"
                    })
                .Build();

        var provider =
            new AppSettingsSmsProviderOptionsProvider(
                configuration);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.GetSettingAsync());
    }
}