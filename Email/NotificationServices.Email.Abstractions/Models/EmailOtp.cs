namespace NotificationServices.Email.Abstractions.Models;

/// <summary>
/// Represents a one-time password email.
/// </summary>
public sealed record EmailOtp(
    string To,
    string Code,
    string? Subject = null);