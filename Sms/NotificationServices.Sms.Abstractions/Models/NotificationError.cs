namespace NotificationServices.Sms.Abstractions.Models;

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

public static class NotificationErrorCodes
{
    public const string InvalidRequest = "InvalidRequest";
    public const string AuthenticationFailed = "AuthenticationFailed";
    public const string Timeout = "Timeout";
    public const string RateLimited = "RateLimited";
    public const string ProviderUnavailable = "ProviderUnavailable";
    public const string InvalidProviderResponse = "InvalidProviderResponse";
    public const string ProviderRejected = "ProviderRejected";
    public const string Unknown = "Unknown";
}
