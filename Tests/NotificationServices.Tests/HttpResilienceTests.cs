using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NotificationServices.DependencyInjection;
using NotificationServices.Sms.Providers;

namespace NotificationServices.Tests;

public sealed class HttpResilienceTests
{
    [Fact]
    public async Task SmsClient_RetriesTransientServerErrors()
    {
        var handler = new SequenceHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
            new HttpResponseMessage(HttpStatusCode.OK));

        using var provider = BuildProvider(handler);
        var client = provider.GetRequiredService<IHttpClientFactory>()
            .CreateClient(nameof(MelipayamakSmsProvider));

        using var response = await client.GetAsync("https://example.test/sms");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, handler.CallCount);
    }

    [Fact]
    public async Task SmsClient_DoesNotRetryClientErrors()
    {
        var handler = new SequenceHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.BadRequest),
            new HttpResponseMessage(HttpStatusCode.OK));

        using var provider = BuildProvider(handler);
        var client = provider.GetRequiredService<IHttpClientFactory>()
            .CreateClient(nameof(MelipayamakSmsProvider));

        using var response = await client.GetAsync("https://example.test/sms");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(1, handler.CallCount);
    }

    private static ServiceProvider BuildProvider(SequenceHttpMessageHandler handler)
    {
        var services = new ServiceCollection();
        services.AddNotificationServices();
        services
            .AddHttpClient(nameof(MelipayamakSmsProvider))
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        return services.BuildServiceProvider();
    }

    private sealed class SequenceHttpMessageHandler(params HttpResponseMessage[] responses)
        : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses = new(responses);

        public int CallCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            CallCount++;

            if (_responses.Count == 0)
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

            return Task.FromResult(_responses.Dequeue());
        }
    }
}
