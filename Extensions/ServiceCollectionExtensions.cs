using Microsoft.Extensions.DependencyInjection;

namespace Jobo.Enterprise.Client.Extensions;

/// <summary>
/// Extension methods for registering <see cref="JoboClient"/> with the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a singleton <see cref="JoboClient"/> with the specified options.
    /// </summary>
    public static IServiceCollection AddJoboClient(
        this IServiceCollection services,
        Action<JoboClientOptions> configure)
    {
        var options = new JoboClientOptions { ApiKey = string.Empty };
        configure(options);

        if (string.IsNullOrWhiteSpace(options.ApiKey))
            throw new ArgumentException("ApiKey is required.", nameof(configure));

        services.AddSingleton(new JoboClient(options));
        return services;
    }

    /// <summary>
    /// Registers a singleton <see cref="JoboClient"/> with the specified API key.
    /// </summary>
    public static IServiceCollection AddJoboClient(
        this IServiceCollection services,
        string apiKey,
        string? baseUrl = null)
    {
        var options = new JoboClientOptions { ApiKey = apiKey };
        if (!string.IsNullOrEmpty(baseUrl))
            options.BaseUrl = baseUrl;

        services.AddSingleton(new JoboClient(options));
        return services;
    }
}
