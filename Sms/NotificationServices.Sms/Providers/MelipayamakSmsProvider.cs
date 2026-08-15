using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.Providers;

public sealed class MelipayamakSmsProvider : ISmsProvider
{
    private const string BodyIdSetting = "BodyId";

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

        var bodyId = _options.GetRequiredProviderSetting(BodyIdSetting);

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

    private async Task<SmsResult> SendRequestAsync(
        string url,
        Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        using var content = new FormUrlEncodedContent(values);
        using var response = await _httpClient.PostAsync(url, content, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("SMS provider returned HTTP {StatusCode}.", (int)response.StatusCode);
            return SmsResult.Failure($"SMS provider returned HTTP {(int)response.StatusCode}.", response.StatusCode.ToString());
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
            _logger.LogError(ex, "SMS provider returned an invalid response.");
            return SmsResult.Failure($"Invalid response from SMS provider: {ex.Message}", "InvalidProviderResponse");
        }

        if (result is null)
        {
            _logger.LogWarning("SMS provider returned an empty response.");
            return SmsResult.Failure("SMS provider returned an empty response.", "EmptyProviderResponse");
        }

        if (result.RetStatus == 1)
        {
            _logger.LogInformation("SMS notification sent successfully.");
            return SmsResult.Success(result.Value);
        }

        _logger.LogWarning("SMS provider rejected the notification with status {ProviderStatus}.", result.RetStatus);
        return SmsResult.Failure(result.Value ?? "SMS provider rejected the request.", result.RetStatus.ToString());
    }

    private sealed class MelipayamakResponse
    {
        public int RetStatus { get; set; }
        public string? Value { get; set; }
    }
}
