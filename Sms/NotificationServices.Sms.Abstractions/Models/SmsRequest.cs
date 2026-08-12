namespace NotificationServices.Sms.Abstractions.Models;

/// <summary>A single SMS message to be sent.</summary>
public class SmsRequest(string mobile, string text)
{
    public string Mobile { get; } = mobile;
    public string Text { get; } = text;
}
