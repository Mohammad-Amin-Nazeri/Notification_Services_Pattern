using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.ConfigurationProviders;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationServices.Email.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the email service, reading settings from appsettings.json
    /// (via <see cref="AppSettingsEmailProviderOptionsProvider"/>).
    /// </summary>
    public static IServiceCollection AddEmailService(this IServiceCollection services)
    {
        services.AddScoped<IEmailProviderOptionsProvider, AppSettingsEmailProviderOptionsProvider>();
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }

    /// <summary>
    /// Registers the email service with a custom <see cref="IEmailProviderOptionsProvider"/>
    /// (for example, a database-backed one).
    /// </summary>
    public static IServiceCollection AddEmailService<TOptionsProvider>(this IServiceCollection services)
        where TOptionsProvider : class, IEmailProviderOptionsProvider
    {
        services.AddScoped<IEmailProviderOptionsProvider, TOptionsProvider>();
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }
}
