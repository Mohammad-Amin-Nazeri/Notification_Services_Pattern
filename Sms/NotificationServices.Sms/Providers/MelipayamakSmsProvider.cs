using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NotificationServices.Abstractions.Errors;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.Providers;

public sealed class MelipayamakSmsProvider : ISmsProvider
{
    private const string BodyIdSetting = "BodyId";
    private const string TemplateBodyIdPrefix = "BodyId:";

    private readonly SmsProviderOptions _options;
    private readonly HttpClient _httpClient;
    private readonly ILogger<MelipayamakSmsProvider> _logger;

    public MelipayamakSmsProvider(
        SmsProviderOptions options,
        HttpClient httpClient,
        ILogger<MelipayamakSmsProvider>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(httpClient);

        _options = options;
        _httpClient = httpClient;
        _logger = logger ?? NullLogger<MelipayamakSmsProvider>.Instance;
    }

    public async Task<SmsResult> SendMessageAsync(
        SmsMessage message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
            throw new InvalidOperationException("SMS provider BaseUrl is not configured.");

        var values = new Dictionary<string, string>
        {
            ["username"] = _options.Username,
            ["password"] = _options.Password,
            ["to"] = message.Mobile,
            ["from"] = _options.From,
            ["text"] = message.Text,
            ["isFlash"] = "false"
        };

        return await SendRequestAsync(_options.BaseUrl, values, cancellationToken);
    }

    public async Task<SmsResult> SendOtpAsync(
        SmsOtp otp,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(otp);

        if (string.IsNullOrWhiteSpace(_options.PatternBaseUrl))
            throw new InvalidOperationException("SMS provider PatternBaseUrl is not configured.");

        var bodyId = ResolveBodyId(otp.TemplateKey);

        var values = new Dictionary<string, string>
        {
            ["username"] = _options.Username,
            ["password"] = _options.Password,
            ["to"] = otp.Mobile,
            ["text"] = $"{otp.Code};",
            ["bodyId"] = bodyId
        };

        return await SendRequestAsync(_options.PatternBaseUrl, values, cancellationToken);
    }

    private string ResolveBodyId(string? templateKey)
    {
        if (!string.IsNullOrWhiteSpace(templateKey) &&
            _options.ProviderSettings.TryGetValue(
                $"{TemplateBodyIdPrefix}{templateKey}",
                out var templateBodyId) &&
            !string.IsNullOrWhiteSpace(templateBodyId))
        {
            return templateBodyId;
        }

        return _options.GetRequiredProviderSetting(BodyIdSetting);
    }

    private async Task<SmsResult> SendRequestAsync(
        string url,
        Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        try
        {
            using var content = new FormUrlEncodedContent(values);
            using var response = await _httpClient.PostAsync(url, content, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = MapHttpStatus(response.StatusCode);
                _logger.LogWarning(
                    "SMS provider returned HTTP {StatusCode} mapped to {ErrorCode}.",
                    (int)response.StatusCode,
                    error.Code);

                return SmsResult.Failure(error);
            }

            MelipayamakResponse? result;

            try
            {
                result = JsonSerializer.Deserialize<MelipayamakResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "SMS provider returned invalid JSON.");
                return SmsResult.Failure(NotificationError.Create(
                    NotificationErrorCodes.InvalidProviderResponse,
                    NotificationErrorCategory.InvalidProviderResponse,
                    "SMS provider returned an invalid response.",
                    false));
            }

            if (result is null)
            {
                _logger.LogWarning("SMS provider returned an empty response.");
                return SmsResult.Failure(NotificationError.Create(
                    NotificationErrorCodes.InvalidProviderResponse,
                    NotificationErrorCategory.InvalidProviderResponse,
                    "SMS provider returned an empty response.",
                    false));
            }

            if (result.RetStatus == 1)
            {
                _logger.LogInformation("SMS notification sent successfully.");
                return SmsResult.Success(result.Value);
            }

            var providerError = NotificationError.Create(
                NotificationErrorCodes.ProviderRejected,
                NotificationErrorCategory.ProviderRejected,
                result.Value ?? "SMS provider rejected the request.",
                false);

            _logger.LogWarning(
                "SMS provider rejected the notification with status {ProviderStatus}.",
                result.RetStatus);

            return SmsResult.Failure(providerError);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            var error = NotificationError.Create(
                NotificationErrorCodes.Timeout,
                NotificationErrorCategory.Timeout,
                "SMS provider request timed out.",
                true);

            _logger.LogWarning("SMS provider request timed out.");
            return SmsResult.Failure(error);
        }
        catch (HttpRequestException ex)
        {
            var error = NotificationError.Create(
                NotificationErrorCodes.ProviderUnavailable,
                NotificationErrorCategory.ProviderUnavailable,
                "SMS provider could not be reached.",
                true);

            _logger.LogError(ex, "SMS provider request failed due to transport error.");
            return SmsResult.Failure(error);
        }
    }

    private static NotificationError MapHttpStatus(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden =>
                NotificationError.Create(
                    NotificationErrorCodes.AuthenticationFailed,
                    NotificationErrorCategory.AuthenticationFailed,
                    "SMS provider authentication failed.",
                    false),

            (HttpStatusCode)429 =>
                NotificationError.Create(
                    NotificationErrorCodes.RateLimited,
                    NotificationErrorCategory.RateLimited,
                    "SMS provider rate limit was exceeded.",
                    true),

            >= HttpStatusCode.InternalServerError =>
                NotificationError.Create(
                    NotificationErrorCodes.ProviderUnavailable,
                    NotificationErrorCategory.ProviderUnavailable,
                    "SMS provider is temporarily unavailable.",
                    true),

            _ => NotificationError.Create(
                NotificationErrorCodes.InvalidRequest,
                NotificationErrorCategory.InvalidRequest,
                $"SMS provider returned HTTP {(int)statusCode}.",
                false)
        };
    }

    private sealed class MelipayamakResponse
    {
        public int RetStatus { get; set; }
        public string? Value { get; set; }
    }
}
