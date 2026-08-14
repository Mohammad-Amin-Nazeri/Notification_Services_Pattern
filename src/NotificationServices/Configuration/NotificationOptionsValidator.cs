using System.Net.Mail;
using NotificationServices.Options;

namespace NotificationServices.Configuration;

internal static class NotificationOptionsValidator
{
    public static void Validate(NotificationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var errors = new List<string>();

        Validate