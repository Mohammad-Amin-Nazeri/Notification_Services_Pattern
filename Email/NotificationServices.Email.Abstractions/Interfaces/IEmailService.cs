using NotificationServices.Email.Abstractions.Models;

namespace NotificationServices.Email.Abstractions.Interfaces;

public interface IEmailService
{
    Task<EmailResult> SendAsync(EmailRequest request);
    Task<EmailResult> SendBulkAsync(IReadOnlyCollection<EmailRequest> requests);
}
