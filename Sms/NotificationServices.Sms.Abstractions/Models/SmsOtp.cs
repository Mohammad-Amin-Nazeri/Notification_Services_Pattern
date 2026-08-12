namespace NotificationServices.Sms.Abstractions.Models;

/// <summary>
/// Represents a one-time password SMS.
/// </summary>
public sealed record SmsOtp(
    string Mobile,
    string Code);