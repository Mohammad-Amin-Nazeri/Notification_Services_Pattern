namespace NotificationServices.Email.Abstractions.Models;

/// <summary>
/// Configuration required by the email provider.
/// </summary>
public sealed class EmailProviderOptions
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public bool EnableSsl { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string FromAddress { get; set; } = string.Empty;

    public string FromName { get; set; } = string.Empty;
}