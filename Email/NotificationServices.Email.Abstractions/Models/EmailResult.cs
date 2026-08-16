using NotificationServices.Abstractions.Errors;

namespace NotificationServices.Email.Abstractions.Models;

public sealed class EmailResult
{
    public bool IsSuccess { get; init; }

    public string? Message { get; init; }

    public string? ErrorCode { get; init; }

    public bool IsRetryable { get; init; }

    public NotificationError? Error { get; init; }

    public static EmailResult Success(string? message = null)
    {
        return new EmailResult
        {
            IsSuccess = true,
            Message = message
        };
    }

    public static EmailResult Failure(
        string message,
        string? errorCode = null,
        bool isRetryable = false,
        NotificationError? error = null)
    {
        return new EmailResult
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = errorCode ?? error?.Code,
            IsRetryable = error?.IsRetryable ?? isRetryable,
            Error = error
        };
    }

    public static EmailResult Failure(NotificationError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return Failure(error.Message, error.Code, error.IsRetryable, error);
    }
}
