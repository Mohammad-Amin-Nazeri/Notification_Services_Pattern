using SharedNotificationError = NotificationServices.Abstractions.Errors.NotificationError;

namespace NotificationServices.Sms.Abstractions.Models;

public sealed class SmsResult
{
    public bool IsSuccess { get; init; }

    public string? Message { get; init; }

    public string? ErrorCode { get; init; }

    public bool IsRetryable { get; init; }

    public SharedNotificationError? Error { get; init; }

    public static SmsResult Success(string? message = null)
    {
        return new SmsResult
        {
            IsSuccess = true,
            Message = message
        };
    }

    public static SmsResult Failure(
        string message,
        string? errorCode = null,
        bool isRetryable = false,
        SharedNotificationError? error = null)
    {
        return new SmsResult
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = errorCode ?? error?.Code,
            IsRetryable = error?.IsRetryable ?? isRetryable,
            Error = error
        };
    }

    public static SmsResult Failure(SharedNotificationError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return Failure(error.Message, error.Code, error.IsRetryable, error);
    }
}
