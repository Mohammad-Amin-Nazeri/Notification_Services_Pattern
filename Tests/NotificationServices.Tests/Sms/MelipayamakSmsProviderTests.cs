using System.Net;
using System.Text;
using NotificationServices.Sms.Abstractions.Models;
using NotificationServices.Sms.Providers;

namespace NotificationServices.Tests.Sms;

public sealed class MelipayamakSmsProviderTests
{
    [Fact]
    public async Task SendMessageAsync_ShouldSendCorrectRequest()
    {
        HttpMethod? capturedMethod = null;
        string? capturedUrl = null;
        string? capturedBody = null;

        var handler = new FakeHttpMessageHandler(
            async request =>
            {
                capturedMethod = request.Method;
                capturedUrl = request.RequestUri?.ToString();
                capturedBody = await request.Content!.ReadAsStringAsync();
                return CreateSuccessResponse("123456");
            });

        using var httpClient = new HttpClient(handler);
        var options = CreateOptions();
        var provider = new MelipayamakSmsProvider(options, httpClient);

        var result = await provider.SendMessageAsync(
            new SmsMessage("09120000000", "Hello World"));

        Assert.True(result.IsSuccess);
        Assert.Equal("123456", result.Message);
        Assert.Equal(HttpMethod.Post, capturedMethod);
        Assert.Equal(options.BaseUrl, capturedUrl);
        Assert.NotNull(capturedBody);
        Assert.Contains("username=test-user", capturedBody);
        Assert.Contains("password=test-password", capturedBody);
        Assert.Contains("to=09120000000", capturedBody);
        Assert.Contains("from=50004001", capturedBody);
        Assert.Contains("text=Hello+World", capturedBody);
        Assert.Contains("isFlash=false", capturedBody);
    }

    [Fact]
    public async Task SendOtpAsync_ShouldUsePatternEndpointAndProviderSetting()
    {
        HttpMethod? capturedMethod = null;
        string? capturedUrl = null;
        string? capturedBody = null;

        var handler = new FakeHttpMessageHandler(
            async request =>
            {
                capturedMethod = request.Method;
                capturedUrl = request.RequestUri?.ToString();
                capturedBody = await request.Content!.ReadAsStringAsync();
                return CreateSuccessResponse("987654");
            });

        using var httpClient = new HttpClient(handler);
        var options = CreateOptions();
        var provider = new MelipayamakSmsProvider(options, httpClient);

        var result = await provider.SendOtpAsync(
            new SmsOtp("09120000000", "123456"));

        Assert.True(result.IsSuccess);
        Assert.Equal("987654", result.Message);
        Assert.Equal(HttpMethod.Post, capturedMethod);
        Assert.Equal(options.PatternBaseUrl, capturedUrl);
        Assert.NotNull(capturedBody);
        Assert.Contains("username=test-user", capturedBody);
        Assert.Contains("password=test-password", capturedBody);
        Assert.Contains("to=09120000000", capturedBody);
        Assert.Contains("bodyId=12345", capturedBody);
        Assert.Contains("text=123456%3B", capturedBody);
    }

    [Fact]
    public async Task SendMessageAsync_WhenProviderReturnsFailure_ShouldReturnFailure()
    {
        var handler = new FakeHttpMessageHandler(
            _ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)));

        using var httpClient = new HttpClient(handler);
        var provider = new MelipayamakSmsProvider(CreateOptions(), httpClient);

        var result = await provider.SendMessageAsync(
            new SmsMessage("09120000000", "Hello"));

        Assert.False(result.IsSuccess);
        Assert.Equal("BadRequest", result.ErrorCode);
    }

    [Fact]
    public async Task SendMessageAsync_WhenResponseIsInvalidJson_ShouldReturnFailure()
    {
        var handler = new FakeHttpMessageHandler(
            _ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("not-json")
            }));

        using var httpClient = new HttpClient(handler);
        var provider = new MelipayamakSmsProvider(CreateOptions(), httpClient);

        var result = await provider.SendMessageAsync(
            new SmsMessage("09120000000", "Hello"));

        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidProviderResponse", result.ErrorCode);
    }

    [Fact]
    public async Task SendMessageAsync_WhenProviderReturnsNullResponse_ShouldReturnFailure()
    {
        var handler = new FakeHttpMessageHandler(
            _ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null")
            }));

        using var httpClient = new HttpClient(handler);
        var provider = new MelipayamakSmsProvider(CreateOptions(), httpClient);

        var result = await provider.SendMessageAsync(
            new SmsMessage("09120000000", "Hello"));

        Assert.False(result.IsSuccess);
        Assert.Equal("EmptyProviderResponse", result.ErrorCode);
    }

    [Fact]
    public async Task SendMessageAsync_WhenProviderRejectsRequest_ShouldReturnFailure()
    {
        var handler = new FakeHttpMessageHandler(
            _ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """
                    {
                        "RetStatus": 0,
                        "Value": "Invalid credentials"
                    }
                    """,
                    Encoding.UTF8,
                    "application/json")
            }));

        using var httpClient = new HttpClient(handler);
        var provider = new MelipayamakSmsProvider(CreateOptions(), httpClient);

        var result = await provider.SendMessageAsync(
            new SmsMessage("09120000000", "Hello"));

        Assert.False(result.IsSuccess);
        Assert.Equal("0", result.ErrorCode);
        Assert.Equal("Invalid credentials", result.Message);
    }

    [Fact]
    public async Task SendMessageAsync_WhenBaseUrlIsMissing_ShouldThrow()
    {
        var options = CreateOptions();
        options.BaseUrl = "";

        using var httpClient = new HttpClient(new FakeHttpMessageHandler(
            _ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));

        var provider = new MelipayamakSmsProvider(options, httpClient);

        await Assert.ThrowsAsync<InvalidOperationException>(() => provider.SendMessageAsync(
            new SmsMessage("09120000000", "Hello")));
    }

    [Fact]
    public async Task SendOtpAsync_WhenPatternUrlIsMissing_ShouldThrow()
    {
        var options = CreateOptions();
        options.PatternBaseUrl = "";

        using var httpClient = new HttpClient(new FakeHttpMessageHandler(
            _ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));

        var provider = new MelipayamakSmsProvider(options, httpClient);

        await Assert.ThrowsAsync<InvalidOperationException>(() => provider.SendOtpAsync(
            new SmsOtp("09120000000", "123456")));
    }

    [Fact]
    public async Task SendOtpAsync_WhenBodyIdIsMissing_ShouldThrow()
    {
        var options = CreateOptions();
        options.ProviderSettings.Remove("BodyId");

        using var httpClient = new HttpClient(new FakeHttpMessageHandler(
            _ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));

        var provider = new MelipayamakSmsProvider(options, httpClient);

        await Assert.ThrowsAsync<InvalidOperationException>(() => provider.SendOtpAsync(
            new SmsOtp("09120000000", "123456")));
    }

    private static SmsProviderOptions CreateOptions()
    {
        return new SmsProviderOptions
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
    }

    private static HttpResponseMessage CreateSuccessResponse(string value)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                $$"""
                {
                    "RetStatus": 1,
                    "Value": "{{value}}"
                }
                """,
                Encoding.UTF8,
                "application/json")
        };
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public FakeHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return _handler(request);
        }
    }
}
