namespace NotificationServices.Email.Abstractions.Models;

/// <summary>
/// Represents a general-purpose email message.
/// </summary>
public sealed record EmailMessage(
    string To,
    string Subject,
    string Body,
    bool IsHtml = true);