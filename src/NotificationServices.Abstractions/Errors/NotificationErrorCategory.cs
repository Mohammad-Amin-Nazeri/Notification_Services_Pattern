namespace NotificationServices.Abstractions.Errors;

public enum NotificationErrorCategory
{
    Unknown,
    InvalidRequest,
    AuthenticationFailed,
    Timeout,
    RateLimited,
    ProviderUnavailable,
    InvalidProviderResponse,
    ProviderRejected
}
