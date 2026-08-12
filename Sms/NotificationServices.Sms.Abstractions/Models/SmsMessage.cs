namespace NotificationServices.Sms.Abstractions.Models;

/// <summary>A single SMS message to be sent.</summary>
/// <summary>
/// Represents a general-purpose SMS message.
/// </summary>
public sealed record SmsMessage(
    string Mobile,
    string Text);