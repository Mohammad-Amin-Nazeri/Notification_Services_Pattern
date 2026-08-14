using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NotificationServices.Sms.Abstractions.Interfaces;
using NotificationServices.Sms.ConfigurationProviders;
using NotificationServices.Sms.Providers;

namespace NotificationServices.Sms.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmsService(
        this IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddScoped<ISmsProviderOptionsProvider, AppSettingsSmsProviderOptionsProvider>();
        services.AddSmsProvider<MelipayamakSmsProvider>("Melipayamak");
        services.AddScoped<ISmsProviderFactory, SmsProviderFactory>();
        services.AddScoped<ISmsService, SmsService>();

        return services;
    }

    public static IServiceCollection AddSmsService<TOptionsProvider>(
        this IServiceCollection services)
        where TOptionsProvider : class, ISmsProviderOptionsProvider
    {
        services.AddHttpClient();

        services.AddScoped<ISmsProviderOptionsProvider, TOptionsProvider>();
        services.AddSmsProvider<MelipayamakSmsProvider>("Melipayamak");
        services.AddScoped<ISmsProviderFactory, SmsProviderFactory>();
        services.AddScoped<ISmsService, SmsService>();

        return services;
    }

    /// <summary>
    /// Registers an SMS provider by a stable runtime key.
    /// Provider selection remains runtime-driven, allowing a configuration provider
    /// to select a different provider for each tenant, user, or license.
    /// </summary>
    public static IServiceCollection AddSmsProvider<TProvider>(
        this IServiceCollection services,
        string providerName)
        where TProvider : class, ISmsProvider
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);

        services.AddTransient<TProvider>();
        services.AddSingleton(
            new SmsProviderRegistration(providerName, typeof(TProvider)));
        services.TryAddSingleton<ISmsProviderRegistry, SmsProviderRegistry>();

        return services;
    }
}
