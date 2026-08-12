namespace NotificationServices.Email.Abstractions.Models;

/// <summary>A single email to be sent (e.g. a verification code).</summary>
public class EmailRequest(string to, string text)
{
    public string To { get; } = to;
    public string Text { get; } = text;
}
