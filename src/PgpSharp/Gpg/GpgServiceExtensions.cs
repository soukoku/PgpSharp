using System;
using PgpSharp;
using PgpSharp.Gpg;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides methods for DI use.
/// </summary>
public static class GpgServiceExtensions
{
    /// <summary>
    /// Adds a singleton <see cref="IPgpTool"/> to services using default configuration options with
    /// customization opportunity.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddGpg(this IServiceCollection services, Action<GpgOptions>? configure = null)
    {
        services.AddSingleton<GpgOptions>(svc =>
        {
            var options = new GpgOptions();
            configure?.Invoke(options);
            return options;
        });
        services.AddSingleton<IPgpTool, GpgTool>();
        return services;
    }

    /// <summary>
    /// Adds a singleton <see cref="IPgpTool"/> to services using the specified configuration options.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddGpg(this IServiceCollection services, GpgOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton<IPgpTool, GpgTool>();
        return services;
    }
}