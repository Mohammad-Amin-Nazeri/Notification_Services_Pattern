namespace NotificationServices.Options;

public sealed class SmsOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Sender { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
}
