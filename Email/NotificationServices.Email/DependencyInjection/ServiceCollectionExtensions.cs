using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Email.ConfigurationProviders;

namespace NotificationServices.Email.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailService(
        this IServiceCollection services)
    {
        services.AddScoped<IEmailProviderOptionsProvider, AppSettingsEmailProviderOptionsProvider>();
        services.TryAddScoped<IEmailTemplateProvider, DefaultEmailTemplateProvider>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    public static IServiceCollection AddEmailService<TOptionsProvider>(
        this IServiceCollection services)
        where TOptionsProvider : class, IEmailProviderOptionsProvider
    {
        services.AddScoped<IEmailProviderOptionsProvider, TOptionsProvider>();
        services.TryAddScoped<IEmailTemplateProvider, DefaultEmailTemplateProvider>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    public static IServiceCollection AddEmailTemplateProvider<TTemplateProvider>(
        this IServiceCollection services)
        where TTemplateProvider : class, IEmailTemplateProvider
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddScoped<IEmailTemplateProvider, TTemplateProvider>();
        return services;
    }
}