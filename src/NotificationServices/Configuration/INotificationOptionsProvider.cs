namespace NotificationServices.Configuration;

/// <summary>
/// Supplies notification provider settings from any application-defined source.
/// </summary>
public interface INotificationOptionsProvider
{
    Task<NotificationOptions> GetOptionsAsync(CancellationToken cancellationToken = default);
}

public sealed class NotificationOptions
{
    public EmailOptions Email { get; init; } = new();
    public SmsOptions Sms { get; init; } = new();
}

public sealed class EmailOptions
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    public bool EnableSsl { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FromAddress { get; init; } = string.Empty;
    public string FromName { get; init; } = string.Empty;
}

public sealed class SmsOptions
{
    public string ProviderType { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string From { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = string.Empty;
    public string PatternBaseUrl { get; init; } = string.Empty;
    public string BodyId { get; init; } = string.Empty;
}
