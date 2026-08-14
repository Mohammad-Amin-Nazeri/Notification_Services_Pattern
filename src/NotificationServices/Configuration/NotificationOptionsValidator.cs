using System.Net.Mail;
using NotificationServices.Options;

namespace NotificationServices.Configuration;

internal static class NotificationOptionsValidator
{
    public static void Validate(NotificationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var errors = new List<string>();

        ValidateEmail(options.Email, errors);
        ValidateSms(options.Sms, errors);

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                "NotificationServices configuration is invalid:\n" +
                string.Join("\n", errors.Select(static error => $"- {error}")));
        }
    }

    private static void ValidateEmail(EmailOptions options, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(options.Host))
            errors.Add("NotificationServices:Email:Host is required.");

        if (options.Port is < 1 or > 65535)
            errors.Add("NotificationServices:Email:Port must be between 1 and 65535.");

        if (string.IsNullOrWhiteSpace(options.FromAddress))
        {
            errors.Add("NotificationServices:Email:FromAddress is required.");
        }
        else if (!MailAddress.TryCreate(options.FromAddress, out _))
        {
            errors.Add("NotificationServices:Email:FromAddress must be a valid email address.");
        }

        if (!string.IsNullOrWhiteSpace(options.Username) && string.IsNullOrWhiteSpace(options.Password))
            errors.Add("NotificationServices:Email:Password is required when Username is configured.");
    }

    private static void ValidateSms(SmsOptions options, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(options.ProviderType))
            errors.Add("NotificationServices:Sms:ProviderType is required.");

        if (string.IsNullOrWhiteSpace(options.Username))
            errors.Add("NotificationServices:Sms:Username is required.");

        if (string.IsNullOrWhiteSpace(options.Password))
            errors.Add("NotificationServices:Sms:Password is required.");

        if (string.IsNullOrWhiteSpace(options.From))
            errors.Add("NotificationServices:Sms:From is required.");

        if (string.IsNullOrWhiteSpace(options.BaseUrl))
            errors.Add("NotificationServices:Sms:BaseUrl is required.");
        else if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _))
            errors.Add("NotificationServices:Sms:BaseUrl must be a valid absolute URI.");

        if (!string.IsNullOrWhiteSpace(options.PatternBaseUrl) &&
            !Uri.TryCreate(options.PatternBaseUrl, UriKind.Absolute, out _))
            errors.Add("NotificationServices:Sms:PatternBaseUrl must be a valid absolute URI.");
    }
}
