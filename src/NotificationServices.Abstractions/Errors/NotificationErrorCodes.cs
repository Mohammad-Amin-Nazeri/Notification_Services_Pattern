namespace NotificationServices.Abstractions.Errors;

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

    // Backward compatibility aliases for previous provider error contracts.
    public const string BadRequest = "BadRequest";
    public const string EmptyProviderResponse = "EmptyProviderResponse";
}
