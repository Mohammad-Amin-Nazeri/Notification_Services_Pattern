using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Configuration;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.DependencyInjection;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.DependencyInjection;

namespace NotificationServices.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationOptionsProvider, AppSettingsNotificationOptionsProvider>();
        return AddNotificationServicesCore(services);
    }

    public static IServiceCollection AddNotificationServices<TOptionsProvider>(this IServiceCollection services)
        where TOptionsProvider : class, INotificationOptionsProvider
    {
        services.AddScoped<INotificationOptionsProvider, TOptionsProvider>();
        return AddNotificationServicesCore(services);
    }

    private static IServiceCollection AddNotificationServicesCore(IServiceCollection services)
    {
        services.AddScoped<IEmailProviderOptionsProvider, EmailOptionsAdapter>();
        services.AddScoped<ISmsProviderOptionsProvider, SmsOptionsAdapter>();

        services.AddEmailService<EmailOptionsAdapter>();
        services.AddSmsService<SmsOptionsAdapter>();

        return services;
    }
}
