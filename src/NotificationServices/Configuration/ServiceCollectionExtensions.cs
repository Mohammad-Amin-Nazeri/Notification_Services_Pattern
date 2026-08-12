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
            services.AddScoped(optionsProviderFactory);
        }
        else
        {
            services.AddScoped<INotificationOptionsProvider, OptionsNotificationOptionsProvider>();
        }

        return services;
    }
}

internal sealed class OptionsNotificationOptionsProvider : INotificationOptionsProvider
{
    public Task<NotificationOptions> GetOptionsAsync(CancellationToken cancellationToken = default)
        => throw new InvalidOperationException(
            "No notification options provider has been configured. Register an application-specific INotificationOptionsProvider or use the configuration integration package.");
}
