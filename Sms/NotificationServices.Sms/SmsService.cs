using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;

namespace NotificationServices.Sms;

/// <summary>
/// Default implementation of <see cref="ISmsService"/>: asks the
/// <see cref="ISmsProviderFactory"/> for the currently configured provider and delegates to it.
/// </summary>
public class SmsService(ISmsProviderFactory factory) : ISmsService
{
    public async Task<SmsResult> SendAsync(SmsRequest request)
    {
        var provider = await factory.GetProviderAsync();
        return await provider.SendAsync(request);
    }

    public async Task<SmsResult> SendBulkAsync(IReadOnlyCollection<SmsRequest> requests)
    {
        var provider = await factory.GetProviderAsync();
        return await provider.SendBulkAsync(requests);
    }
}
