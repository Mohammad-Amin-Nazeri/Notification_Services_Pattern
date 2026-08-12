using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.ConfigurationProviders;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationServices.Sms.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the SMS service, reading settings from appsettings.json
    /// (via <see cref="AppSettingsSmsProviderOptionsProvider"/>).
    /// </summary>
    public static IServiceCollection AddSmsService(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<ISmsProviderOptionsProvider, AppSettingsSmsProviderOptionsProvider>();
        services.AddScoped<ISmsProviderFactory, SmsProviderFactory>();
        services.AddScoped<ISmsService, SmsService>();
        return services;
    }

    /// <summary>
    /// Registers the SMS service with a custom <see cref="ISmsProviderOptionsProvider"/>
    /// (for example, a database-backed one). This is the single line you need to change
    /// if the settings source ever changes - nothing else in the library is affected.
    /// </summary>
    public static IServiceCollection AddSmsService<TOptionsProvider>(this IServiceCollection services)
        where TOptionsProvider : class, ISmsProviderOptionsProvider
    {
        services.AddHttpClient();
        services.AddScoped<ISmsProviderOptionsProvider, TOptionsProvider>();
        services.AddScoped<ISmsProviderFactory, SmsProviderFactory>();
        services.AddScoped<ISmsService, SmsService>();
        return services;
    }
}
