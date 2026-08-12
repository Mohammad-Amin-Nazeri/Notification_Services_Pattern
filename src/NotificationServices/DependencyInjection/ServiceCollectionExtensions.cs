using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Configuration;
using NotificationServices.Email.DependencyInjection;
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
        services.AddEmailService<EmailOptionsAdapter>();
        services.AddSmsService<SmsOptionsAdapter>();
        return services;
    }
}
