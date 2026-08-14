namespace NotificationServices.Sms;

internal sealed record SmsProviderRegistration(
    string Name,
    Type ProviderType);
