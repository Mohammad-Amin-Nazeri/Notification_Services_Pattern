// Minimal example showing how to wire up and use both services.
// Run with: dotnet run --project samples/NotificationServices.Sample
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.Abstractions.Models;
using NotificationServices.Email.DependencyInjection;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Models;
using NotificationServices.Sms.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);

// Both services read their settings from appsettings.json by default.
// To switch either one to a database (or any other source) later, implement
// ISmsProviderOptionsProvider / IEmailProviderOptionsProvider yourself and register it with:
//   services.AddSmsService<MyDbSmsProviderOptionsProvider>();
//   services.AddEmailService<MyDbEmailProviderOptionsProvider>();
services.AddSmsService();
services.AddEmailService();

await using var provider = services.BuildServiceProvider();

var smsService = provider.GetRequiredService<ISmsService>();
var smsResult = await smsService.SendAsync(new SmsRequest("09120000000", "1234"));
Console.WriteLine($"SMS  -> success: {smsResult.IsSuccess}, message: {smsResult.Message}");

var emailService = provider.GetRequiredService<IEmailService>();
var emailResult = await emailService.SendAsync(new EmailRequest("someone@example.com", "1234"));
Console.WriteLine($"Email -> success: {emailResult.IsSuccess}, message: {emailResult.Message}");
