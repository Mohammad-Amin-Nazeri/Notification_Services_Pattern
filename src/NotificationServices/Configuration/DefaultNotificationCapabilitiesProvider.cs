namespace NotificationServices.Configuration;

/// <summary>
/// Default capability provider used when the host application does not supply
/// tenant or license-specific capability resolution.
/// </summary>
public sealed class DefaultNotificationCapabilitiesProvider : INotificationCapabilitiesProvider
{
    private static readonly NotificationCapabilities Capabilities = new();

    public ValueTask<NotificationCapabilities> GetCapabilitiesAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(Capabilities);
    }
}
