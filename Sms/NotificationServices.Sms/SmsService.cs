using NotificationServices.Configuration;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms;

public sealed class SmsService(
    ISmsProviderFactory factory,
    INotificationCapabilitiesProvider capabilitiesProvider) : ISmsService
{
    private const string CapabilityDisabledCode = "sms.disabled";

    public async Task<SmsResult> SendMessageAsync(
        SmsMessage message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        ValidateMessage(message);

        if (!await IsEnabledAsync(cancellationToken))
            return SmsResult.Failure("SMS notifications are disabled for the current context.", CapabilityDisabledCode);

        var provider = await factory.GetProviderAsync(cancellationToken);

        return await provider.SendMessageAsync(message, cancellationToken);
    }

    public async Task<SmsResult> SendOtpAsync(
        SmsOtp otp,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(otp);
        ValidateOtp(otp);

        if (!await IsEnabledAsync(cancellationToken))
            return SmsResult.Failure("SMS notifications are disabled for the current context.", CapabilityDisabledCode);

        var provider = await factory.GetProviderAsync(cancellationToken);

        return await provider.SendOtpAsync(otp, cancellationToken);
    }

    private async ValueTask<bool> IsEnabledAsync(CancellationToken cancellationToken)
        => (await capabilitiesProvider.GetCapabilitiesAsync(cancellationToken)).SmsEnabled;

    private static void ValidateMessage(SmsMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.Mobile))
            throw new ArgumentException("Mobile number is required.", nameof(message));

        if (string.IsNullOrWhiteSpace(message.Text))
            throw new ArgumentException("SMS text is required.", nameof(message));
    }

    private static void ValidateOtp(SmsOtp otp)
    {
        if (string.IsNullOrWhiteSpace(otp.Mobile))
            throw new ArgumentException("Mobile number is required.", nameof(otp));

        if (string.IsNullOrWhiteSpace(otp.Code))
            throw new ArgumentException("OTP code is required.", nameof(otp));
    }
}
