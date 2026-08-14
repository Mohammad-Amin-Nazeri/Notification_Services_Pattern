namespace NotificationServices.Configuration;

/// <summary>
/// Describes which notification capabilities are enabled for the current application context.
/// Implementations may resolve capabilities from a tenant, license, user, subscription, database,
/// or any other application-owned source.
/// </summary>
public interface INotificationCapabilitiesProvider
{
    ValueTask<NotificationCapabilities> GetCapabilitiesAsync(
        CancellationToken cancellationToken = default);
}

public sealed class NotificationCapabilities
{
    public bool EmailEnabled { get; init; } = true;
    public bool SmsEnabled { get; init; } = true;
}
