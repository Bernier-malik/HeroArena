using System;
using System.IO;

namespace HeroArena.Data;

public static class DatabaseSettings
{
    private const string DefaultConnection =
        "Server=localhost,1433;Database=ExerciceHero;User Id=sa;Password=Your_password123;TrustServerCertificate=True;";
    private static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "db-connection.txt");

    public static string GetConnectionString()
    {
        var environmentConnection = Environment.GetEnvironmentVariable("HEROARENA_DB_CONNECTION");
        if (!string.IsNullOrWhiteSpace(environmentConnection))
        {
            return environmentConnection;
        }

        if (File.Exists(ConfigPath))
        {
            var fileValue = File.ReadAllText(ConfigPath).Trim();
            if (!string.IsNullOrWhiteSpace(fileValue))
            {
                return fileValue;
            }
        }

        return DefaultConnection;
    }

    public static void SaveConnectionString(string connectionString)
    {
        File.WriteAllText(ConfigPath, connectionString.Trim());
    }
}
