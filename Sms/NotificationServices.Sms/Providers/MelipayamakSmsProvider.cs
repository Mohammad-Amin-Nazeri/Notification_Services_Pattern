using System.Text.Json;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms.Providers;

/// <summary>
/// SAMPLE / REFERENCE PROVIDER.
///
/// This is a working implementation of <see cref="ISmsProvider"/> for the Melipayamak.com
/// SMS gateway, included as an example of how a real provider plugs into this library.
/// It is a good template to copy from when adding support for another gateway
/// (Kavenegar, Twilio, SMS.ir, ...): implement <see cref="ISmsProvider"/>, add a value to
/// <c>SmsProviderType</c>, and wire it up in <c>SmsProviderFactory</c>.
/// </summary>
public class MelipayamakSmsProvider : ISmsProvider
{
    private readonly SmsProviderOptions _options;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public MelipayamakSmsProvider(SmsProviderOptions options, HttpClient httpClient, string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("Melipayamak base URL must be configured.", nameof(baseUrl));

        _options = options;
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    public async Task<SmsResult> SendAsync(SmsRequest request)
    {
        var response = await SendInternalAsync(request);
        return new SmsResult
        {
            IsSuccess = response?.RetStatus == 1,
            Message = response?.Value
        };
    }

    public async Task<SmsResult> SendBulkAsync(IReadOnlyCollection<SmsRequest> requests)
    {
        foreach (var request in requests)
        {
            var response = await SendInternalAsync(request);
            if (response?.RetStatus != 1)
                return new SmsResult { IsSuccess = false, Message = response?.Value };
        }

        return new SmsResult { IsSuccess = true };
    }

    private async Task<MelipayamakResponse?> SendInternalAsync(SmsRequest request)
    {
        var values = new Dictionary<string, string>
        {
            ["username"] = _options.Username,
            ["password"] = _options.Password,
            ["to"] = request.Mobile,
            ["from"] = _options.From,
            ["text"] = $"{request.Text};",
            ["bodyId"] = _options.BodyId
        };

        using var content = new FormUrlEncodedContent(values);
        var httpResponse = await _httpClient.PostAsync(_baseUrl, content);
        if (!httpResponse.IsSuccessStatusCode)
            return null;

        var json = await httpResponse.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MelipayamakResponse>(json);
    }

    private sealed class MelipayamakResponse
    {
        public int RetStatus { get; set; }
        public string? Value { get; set; }
    }
}
