using System.Text.Json;

namespace TripMate.Helpers;

/// <summary>
/// Database configuration. Uses SQLite on Android/iOS (local), SQL Server on Windows.
/// </summary>
public static class DatabaseConfiguration
{
    private const string SqlServerDefault =
        "Server=.;Database=TripMate;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

    /// <summary>
    /// Returns true if running on Android or iOS (use SQLite).
    /// </summary>
    public static bool UseSqlite =>
        DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS;

    /// <summary>
    /// SQLite database path for mobile. Only valid when UseSqlite is true.
    /// </summary>
    public static string SqlitePath =>
        Path.Combine(FileSystem.AppDataDirectory, "tripmate.db3");

    /// <summary>
    /// SQL Server connection string. Only used on Windows.
    /// </summary>
    public static string GetSqlServerConnectionString()
    {
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

        return SqlServerDefault;
    }

    public static string GetConnectionString() =>
        UseSqlite ? $"Filename={SqlitePath}" : GetSqlServerConnectionString();

    /// <summary>
    /// Use API backend instead of local database. Set via appsettings "UseApiBackend": true.
    /// </summary>
    public static bool UseApiBackend => GetBoolConfig("UseApiBackend", false);

    /// <summary>
    /// API base URL (e.g. http://10.0.2.2:5000 for Android emulator, http://localhost:5000 for Windows).
    /// </summary>
    public static string ApiBaseUrl => GetStringConfig("ApiBaseUrl", "http://localhost:5000");

    private static string GetStringConfig(string key, string defaultValue)
    {
        try
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").GetAwaiter().GetResult();
            using var reader = new StreamReader(stream);
            var doc = JsonDocument.Parse(reader.ReadToEnd());
            if (doc.RootElement.TryGetProperty(key, out var prop))
                return prop.GetString() ?? defaultValue;
        }
        catch { }
        return defaultValue;
    }

    private static bool GetBoolConfig(string key, bool defaultValue)
    {
        try
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").GetAwaiter().GetResult();
            using var reader = new StreamReader(stream);
            var doc = JsonDocument.Parse(reader.ReadToEnd());
            if (doc.RootElement.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.True)
                return true;
            if (prop.ValueKind == JsonValueKind.False) return false;
        }
        catch { }
        return defaultValue;
    }
}
