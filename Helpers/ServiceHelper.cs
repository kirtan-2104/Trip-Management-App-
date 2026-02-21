using Microsoft.Extensions.DependencyInjection;

namespace TripMate.Helpers;

/// <summary>
/// Service locator for resolving dependencies when constructor injection is not available (e.g. Shell DataTemplate).
/// </summary>
public static class ServiceHelper
{
    public static IServiceProvider? Services { get; set; }

    public static T GetRequiredService<T>() where T : notnull
    {
        if (Services == null)
            throw new InvalidOperationException("ServiceProvider has not been initialized.");
        return Services.GetRequiredService<T>();
    }
}
