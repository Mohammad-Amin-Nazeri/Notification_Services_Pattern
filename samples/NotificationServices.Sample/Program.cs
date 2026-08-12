using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.Abstractions.Models;
using NotificationServices.Email.DependencyInjection;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;
using NotificationServices.Sms.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: false)
    .Build();

var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(configuration);

services.AddEmailService();
services.AddSmsService();

await using var provider = services.BuildServiceProvider();

var smsService = provider.GetRequiredService<ISmsService>();

var smsMessageResult = await smsService.SendMessageAsync(
    new SmsMessage(
        "09120000000",
        "This is a test message."));

Console.WriteLine(
    $"SMS Message -> Success: {smsMessageResult.IsSuccess}, " +
    $"Message: {smsMessageResult.Message}");

var smsOtpResult = await smsService.SendOtpAsync(
    new SmsOtp(
        "09120000000",
        "1234"));

Console.WriteLine(
    $"SMS OTP -> Success: {smsOtpResult.IsSuccess}, " +
    $"Message: {smsOtpResult.Message}");

var emailService = provider.GetRequiredService<IEmailService>();

var emailMessageResult = await emailService.SendMessageAsync(
    new EmailMessage(
        "someone@example.com",
        "Test Email",
        "<h1>Hello!</h1><p>This is a test email.</p>",
        IsHtml: true));

Console.WriteLine(
    $"Email Message -> Success: {emailMessageResult.IsSuccess}, " +
    $"Message: {emailMessageResult.Message}");

var emailOtpResult = await emailService.SendOtpAsync(
    new EmailOtp(
        "someone@example.com",
        "1234"));

Console.WriteLine(
    $"Email OTP -> Success: {emailOtpResult.IsSuccess}, " +
    $"Message: {emailOtpResult.Message}");