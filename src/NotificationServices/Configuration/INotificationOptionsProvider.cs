using NotificationServices.Options;

namespace NotificationServices.Configuration;

/// <summary>
/// Supplies notification settings from any application-defined source.
/// The library does not know whether the source is appsettings, database, cache, API, or another system.
/// </summary>
public interface INotificationOptionsProvider
{
    ValueTask<NotificationOptions> GetOptionsAsync(CancellationToken cancellationToken = default);
}

public sealed class NotificationOptions
{
    public EmailOptions Email { get; init; } = new();
    public SmsOptions Sms { get; init; } = new();
}
