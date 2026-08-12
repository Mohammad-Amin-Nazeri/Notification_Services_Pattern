using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.ConfigurationProviders;

namespace NotificationServices.Sms.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmsService(
        this IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddScoped<
            ISmsProviderOptionsProvider,
            AppSettingsSmsProviderOptionsProvider>();

        services.AddScoped<ISmsProviderFactory, SmsProviderFactory>();

        services.AddScoped<ISmsService, SmsService>();

        return services;
    }

    public static IServiceCollection AddSmsService<TOptionsProvider>(
        this IServiceCollection services)
        where TOptionsProvider : class, ISmsProviderOptionsProvider
    {
        services.AddHttpClient();

        services.AddScoped<
            ISmsProviderOptionsProvider,
            TOptionsProvider>();

        services.AddScoped<ISmsProviderFactory, SmsProviderFactory>();

        services.AddScoped<ISmsService, SmsService>();

        return services;
    }
}