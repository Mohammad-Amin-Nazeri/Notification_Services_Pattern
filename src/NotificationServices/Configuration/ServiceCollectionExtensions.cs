using Microsoft.Extensions.DependencyInjection;

namespace NotificationServices.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationServices(
        this IServiceCollection services,
        Func<IServiceProvider, INotificationOptionsProvider>? optionsProviderFactory = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (optionsProviderFactory is not null)
        {
            services.AddScoped<INotificationOptionsProvider>(optionsProviderFactory);
        }

        return services;
    }
}
