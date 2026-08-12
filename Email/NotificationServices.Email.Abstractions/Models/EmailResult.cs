namespace NotificationServices.Email.Abstractions.Models;

public sealed class EmailResult
{
    public bool IsSuccess { get; init; }

    public string? Message { get; init; }

    public string? ErrorCode { get; init; }

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
        string? errorCode = null)
    {
        return new EmailResult
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = errorCode
        };
    }
}