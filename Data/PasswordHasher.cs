using System;
using System.Security.Cryptography;
using System.Text;

namespace HeroArena.Data;

public static class PasswordHasher
{
    public static string ToBase64Sha256(string rawValue)
    {
        var bytes = Encoding.UTF8.GetBytes(rawValue);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
