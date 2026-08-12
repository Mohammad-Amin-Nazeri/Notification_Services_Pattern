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
        HttpRequestMessage? capturedRequest = null;

        var handler = new FakeHttpMessageHandler(
            request =>
            {
                capturedRequest = request;

                return new HttpResponseMessage(
                    HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        """
                        {
                            "RetStatus": 1,
                            "Value": "123456"
                        }
                        """,
                        Encoding.UTF8,
                        "application/json")
                };
            });

        using var httpClient = new HttpClient(handler);

        var options = CreateOptions();

        var provider = new MelipayamakSmsProvider(
            options,
            httpClient);

        var message = new SmsMessage(
            "09120000000",
            "Hello World");

        var result = await provider.SendMessageAsync(
            message);

        Assert.True(result.IsSuccess);
        Assert.Equal(
            "123456",
            result.Message);

        Assert.NotNull(capturedRequest);

        Assert.Equal(
            HttpMethod.Post,
            capturedRequest!.Method);

        Assert.Equal(
            options.BaseUrl,
            capturedRequest.RequestUri!.ToString());

        var body =
            await capturedRequest.Content!.ReadAsStringAsync();

        Assert.Contains(
            "username=test-user",
            body);

        Assert.Contains(
            "password=test-password",
            body);

        Assert.Contains(
            "to=09120000000",
            body);

        Assert.Contains(
            "from=50004001",
            body);

        Assert.Contains(
            "text=Hello",
            body);
    }

    [Fact]
    public async Task SendOtpAsync_ShouldUsePatternEndpoint()
    {
        HttpRequestMessage? capturedRequest = null;

        var handler = new FakeHttpMessageHandler(
            request =>
            {
                capturedRequest = request;

                return new HttpResponseMessage(
                    HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        """
                        {
                            "RetStatus": 1,
                            "Value": "987654"
                        }
                        """,
                        Encoding.UTF8,
                        "application/json")
                };
            });

        using var httpClient = new HttpClient(handler);

        var options = CreateOptions();

        var provider = new MelipayamakSmsProvider(
            options,
            httpClient);

        var otp = new SmsOtp(
            "09120000000",
            "123456");

        var result = await provider.SendOtpAsync(
            otp);

        Assert.True(result.IsSuccess);
        Assert.Equal(
            "987654",
            result.Message);

        Assert.NotNull(capturedRequest);

        Assert.Equal(
            options.PatternBaseUrl,
            capturedRequest!.RequestUri!.ToString());

        var body =
            await capturedRequest.Content!.ReadAsStringAsync();

        Assert.Contains(
            "to=09120000000",
            body);

        Assert.Contains(
            "bodyId=12345",
            body);

        Assert.Contains(
            "text=123456%3B",
            body);
    }

    [Fact]
    public async Task SendMessageAsync_WhenProviderReturnsFailure_ShouldReturnFailure()
    {
        var handler = new FakeHttpMessageHandler(
            _ =>
                new HttpResponseMessage(
                    HttpStatusCode.BadRequest));

        using var httpClient = new HttpClient(handler);

        var provider = new MelipayamakSmsProvider(
            CreateOptions(),
            httpClient);

        var result = await provider.SendMessageAsync(
            new SmsMessage(
                "09120000000",
                "Hello"));

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "BadRequest",
            result.ErrorCode);
    }

    [Fact]
    public async Task SendMessageAsync_WhenResponseIsInvalidJson_ShouldReturnFailure()
    {
        var handler = new FakeHttpMessageHandler(
            _ =>
                new HttpResponseMessage(
                    HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        "not-json")
                });

        using var httpClient = new HttpClient(handler);

        var provider = new MelipayamakSmsProvider(
            CreateOptions(),
            httpClient);

        var result = await provider.SendMessageAsync(
            new SmsMessage(
                "09120000000",
                "Hello"));

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "InvalidProviderResponse",
            result.ErrorCode);
    }

    [Fact]
    public async Task SendMessageAsync_WhenProviderReturnsNullResponse_ShouldReturnFailure()
    {
        var handler = new FakeHttpMessageHandler(
            _ =>
                new HttpResponseMessage(
                    HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        "null")
                });

        using var httpClient = new HttpClient(handler);

        var provider = new MelipayamakSmsProvider(
            CreateOptions(),
            httpClient);

        var result = await provider.SendMessageAsync(
            new SmsMessage(
                "09120000000",
                "Hello"));

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "EmptyProviderResponse",
            result.ErrorCode);
    }

    [Fact]
    public async Task SendMessageAsync_WhenProviderRejectsRequest_ShouldReturnFailure()
    {
        var handler = new FakeHttpMessageHandler(
            _ =>
                new HttpResponseMessage(
                    HttpStatusCode.OK)
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
                });

        using var httpClient = new HttpClient(handler);

        var provider = new MelipayamakSmsProvider(
            CreateOptions(),
            httpClient);

        var result = await provider.SendMessageAsync(
            new SmsMessage(
                "09120000000",
                "Hello"));

        Assert.False(result.IsSuccess);

        Assert.Equal(
            "0",
            result.ErrorCode);

        Assert.Equal(
            "Invalid credentials",
            result.Message);
    }

    [Fact]
    public async Task SendMessageAsync_WhenBaseUrlIsMissing_ShouldThrow()
    {
        var options = CreateOptions();
        options.BaseUrl = "";

        using var httpClient = new HttpClient(
            new FakeHttpMessageHandler(
                _ => new HttpResponseMessage(
                    HttpStatusCode.OK)));

        var provider = new MelipayamakSmsProvider(
            options,
            httpClient);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.SendMessageAsync(
                new SmsMessage(
                    "09120000000",
                    "Hello")));
    }

    [Fact]
    public async Task SendOtpAsync_WhenPatternUrlIsMissing_ShouldThrow()
    {
        var options = CreateOptions();
        options.PatternBaseUrl = "";

        using var httpClient = new HttpClient(
            new FakeHttpMessageHandler(
                _ => new HttpResponseMessage(
                    HttpStatusCode.OK)));

        var provider = new MelipayamakSmsProvider(
            options,
            httpClient);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.SendOtpAsync(
                new SmsOtp(
                    "09120000000",
                    "123456")));
    }

    [Fact]
    public async Task SendOtpAsync_WhenBodyIdIsMissing_ShouldThrow()
    {
        var options = CreateOptions();
        options.BodyId = "";

        using var httpClient = new HttpClient(
            new FakeHttpMessageHandler(
                _ => new HttpResponseMessage(
                    HttpStatusCode.OK)));

        var provider = new MelipayamakSmsProvider(
            options,
            httpClient);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.SendOtpAsync(
                new SmsOtp(
                    "09120000000",
                    "123456")));
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
            BodyId = "12345"
        };
    }

    private sealed class FakeHttpMessageHandler(
        Func<HttpRequestMessage, HttpResponseMessage> handler)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                handler(request));
        }
    }
}