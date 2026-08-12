using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationServices.DependencyInjection;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Interfaces;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

var services = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
    .AddNotificationServices();

await using var provider = services.BuildServiceProvider();

Console.WriteLine($"Email service resolved: {provider.GetRequiredService<IEmailService>() is not null}");
Console.WriteLine($"SMS service resolved: {provider.GetRequiredService<ISmsService>() is not null}");
Console.WriteLine("Notification services configured successfully.");
