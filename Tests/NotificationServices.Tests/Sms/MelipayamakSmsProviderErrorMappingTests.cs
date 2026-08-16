using System.Net;
using System.Text;
using NotificationServices.Abstractions.Errors;
using NotificationServices.Sms.Abstractions.Models;
using NotificationServices.Sms.Providers;

namespace NotificationServices.Tests.Sms;

public sealed class MelipayamakSmsProviderErrorMappingTests
{
    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, NotificationErrorCodes.AuthenticationFailed, NotificationErrorCategory.AuthenticationFailed, false)]
    [InlineData(HttpStatusCode.Forbidden, NotificationErrorCodes.AuthenticationFailed, NotificationErrorCategory.AuthenticationFailed, false)]
    [InlineData(HttpStatusCode.TooManyRequests, NotificationErrorCodes.RateLimited, NotificationErrorCategory.RateLimited, true)]
    [InlineData(HttpStatusCode.InternalServerError, NotificationErrorCodes.ProviderUnavailable, NotificationErrorCategory.ProviderUnavailable, true)]
    [InlineData(HttpStatusCode.BadRequest, NotificationErrorCodes.InvalidRequest, NotificationErrorCategory.InvalidRequest, false)]
    public async Task SendMessageAsync_WhenHttpFailure_ShouldMapToSharedError(
        HttpStatusCode statusCode,
        string expectedCode,
        NotificationErrorCategory expectedCategory,
        bool expectedRetryable)
    {
        var handler = new MelipayamakTestHttpMessageHandler(
            _ => Task.FromResult(new HttpResponseMessage(statusCode)));

        using var httpClient = new HttpClient(handler);
        var provider = new MelipayamakSmsProvider(CreateOptions(), httpClient);

        var result = await provider.SendMessageAsync(
            new SmsMessage("09120000000", "Hello"));

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal(expectedCode, result.Error.Code);
        Assert.Equal(expectedCategory, result.Error.Category);
        Assert.Equal(expectedRetryable, result.Error.IsRetryable);
        Assert.Equal(expectedCode, result.ErrorCode);
        Assert.Equal(expectedRetryable, result.IsRetryable);
    }

    [Fact]
    public async Task SendMessageAsync_WhenResponseIsInvalidJson_ShouldMapToInvalidProviderResponse()
    {
        var handler = new MelipayamakTestHttpMessageHandler(
            _ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("not-json", Encoding.UTF8, "application/json")
            }));

        using var httpClient = new HttpClient(handler);
        var provider = new MelipayamakSmsProvider(CreateOptions(), httpClient);

        var result = await provider.SendMessageAsync(
            new SmsMessage("09120000000", "Hello"));

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal(NotificationErrorCodes.InvalidProviderResponse, result.Error.Code);
        Assert.Equal(NotificationErrorCategory.InvalidProviderResponse, result.Error.Category);
        Assert.False(result.Error.IsRetryable);
    }

    [Fact]
    public async Task SendMessageAsync_WhenProviderRejectsRequest_ShouldMapToProviderRejected()
    {
        var handler = new MelipayamakTestHttpMessageHandler(
            _ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"RetStatus\":0,\"Value\":\"Invalid credentials\"}",
                    Encoding.UTF8,
                    "application/json")
            }));

        using var httpClient = new HttpClient(handler);
        var provider = new MelipayamakSmsProvider(CreateOptions(), httpClient);

        var result = await provider.SendMessageAsync(
            new SmsMessage("09120000000", "Hello"));

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal(NotificationErrorCodes.ProviderRejected, result.Error.Code);
        Assert.Equal(NotificationErrorCategory.ProviderRejected, result.Error.Category);
        Assert.False(result.Error.IsRetryable);
        Assert.Equal("Invalid credentials", result.Error.Message);
    }

    private static SmsProviderOptions CreateOptions()
        => new()
        {
            ProviderType = "Melipayamak",
            Username = "test-user",
            Password = "test-password",
            From = "50004001",
            BaseUrl = "https://example.com/send",
            PatternBaseUrl = "https://example.com/pattern",
            ProviderSettings = new Dictionary<string, string>
            {
                ["BodyId"] = "12345"
            }
        };

    private sealed class MelipayamakTestHttpMessageHandler
        : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public MelipayamakTestHttpMessageHandler(
            Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => _handler(request);
    }
}
