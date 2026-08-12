namespace NotificationServices.Email.Abstractions.Models;

public class EmailResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}
