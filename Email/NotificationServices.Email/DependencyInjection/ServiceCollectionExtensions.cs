using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.ConfigurationProviders;

namespace NotificationServices.Email.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailService(
        this IServiceCollection services)
    {
        services.AddScoped<
            IEmailProviderOptionsProvider,
            AppSettingsEmailProviderOptionsProvider>();

        services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    public static IServiceCollection AddEmailService<TOptionsProvider>(
        this IServiceCollection services)
        where TOptionsProvider : class, IEmailProviderOptionsProvider
    {
        services.AddScoped<
            IEmailProviderOptionsProvider,
            TOptionsProvider>();

        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}