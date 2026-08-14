namespace NotificationServices.Sms;

public sealed record SmsProviderRegistration(
    string Name,
    Type ProviderType);
