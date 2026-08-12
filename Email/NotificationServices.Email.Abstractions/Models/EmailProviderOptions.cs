namespace NotificationServices.Email.Abstractions.Models;

/// <summary>
/// SMTP settings needed to send email. By default bound from the "EmailProvider" section
/// of your configuration (see <c>AppSettingsEmailProviderOptionsProvider</c>), but any
/// <c>IEmailProviderOptionsProvider</c> implementation (e.g. database-backed) can supply it.
/// </summary>
public class EmailProviderOptions
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FromAddress { get; set; } = null!;
    public string FromName { get; set; } = null!;
}
