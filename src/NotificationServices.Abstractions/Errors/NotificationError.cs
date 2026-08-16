namespace NotificationServices.Abstractions.Errors;

public sealed record NotificationError
{
    public required string Code { get; init; }

    public required NotificationErrorCategory Category { get; init; }

    public required string Message { get; init; }

    public bool IsRetryable { get; init; }

    public static NotificationError Create(
        string code,
        NotificationErrorCategory category,
        string message,
        bool isRetryable = false)
    {
        return new NotificationError
        {
            Code = code,
            Category = category,
            Message = message,
            IsRetryable = isRetryable
        };
    }
}
