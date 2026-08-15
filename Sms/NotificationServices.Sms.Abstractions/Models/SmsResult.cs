namespace NotificationServices.Sms.Abstractions.Models;

public sealed class SmsResult
{
    public bool IsSuccess { get; init; }

    public string? Message { get; init; }

    public string? ErrorCode { get; init; }

    public bool IsRetryable { get; init; }

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
        bool isRetryable = false)
    {
        return new SmsResult
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = errorCode,
            IsRetryable = isRetryable
        };
    }
}