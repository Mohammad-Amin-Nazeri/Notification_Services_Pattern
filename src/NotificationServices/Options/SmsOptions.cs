namespace NotificationServices.Options;

public sealed class SmsOptions
{
    public string ProviderType { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string PatternBaseUrl { get; set; } = string.Empty;
    public string BodyId { get; set; } = string.Empty;
}
