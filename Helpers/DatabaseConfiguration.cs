using System.Text.Json;

namespace TripMate.Helpers;

/// <summary>
/// Loads SQL Server connection string from appsettings.json or environment.
/// </summary>
public static class DatabaseConfiguration
{
    private const string DefaultConnectionString =
        "Server=.;Database=TripMate;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

    public static string GetConnectionString()
    {
        // Environment variable override (for production/CI)
        var envConnection = Environment.GetEnvironmentVariable("TRIPMATE_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(envConnection))
            return envConnection;

        try
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").GetAwaiter().GetResult();
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("ConnectionStrings", out var connSection) &&
                connSection.TryGetProperty("DefaultConnection", out var connProp))
            {
                var connStr = connProp.GetString();
                if (!string.IsNullOrWhiteSpace(connStr))
                    return connStr;
            }
        }
        catch
        {
            // Fall through to default
        }

        return DefaultConnectionString;
    }
}
