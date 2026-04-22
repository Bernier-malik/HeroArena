using System;

namespace HeroArena.Data;

public static class DatabaseSettings
{
    private const string DefaultConnection =
        "Server=localhost,1433;Database=ExerciceHero;User Id=sa;Password=Your_password123;TrustServerCertificate=True;";

    public static string GetConnectionString()
    {
        return Environment.GetEnvironmentVariable("HEROARENA_DB_CONNECTION") ?? DefaultConnection;
    }
}
