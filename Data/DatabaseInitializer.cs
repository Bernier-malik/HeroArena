using System;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace HeroArena.Data;

public static class DatabaseInitializer
{
    public static string? LastInitializationError { get; private set; }

    public static void Initialize()
    {
        try
        {
            LastInitializationError = null;
            using var db = new HeroArenaDbContext();
            db.Database.EnsureCreated();

            if (db.Logins.Any())
            {
                return;
            }

            var login = new LoginEntity
            {
                Username = "admin",
                PasswordHash = PasswordHasher.ToBase64Sha256("admin123")
            };

            var player = new PlayerEntity
            {
                Name = "Player1",
                Login = login
            };

            var warrior = new HeroEntity { Name = "Warrior", Health = 120 };
            var mage = new HeroEntity { Name = "Mage", Health = 90 };

            var slash = new SpellEntity { Name = "Slash", Damage = 20, Description = "Attaque de base du guerrier." };
            var shieldBash = new SpellEntity { Name = "Shield Bash", Damage = 15, Description = "Coup de bouclier qui etourdit." };
            var fireball = new SpellEntity { Name = "Fireball", Damage = 28, Description = "Boule de feu puissante." };
            var frostBolt = new SpellEntity { Name = "Frost Bolt", Damage = 18, Description = "Projectile de glace." };

            db.Players.Add(player);
            db.Heroes.AddRange(warrior, mage);
            db.Spells.AddRange(slash, shieldBash, fireball, frostBolt);
            db.SaveChanges();

            db.PlayerHeroes.Add(new PlayerHeroEntity { PlayerId = player.Id, HeroId = warrior.Id });
            db.HeroSpells.AddRange(
                new HeroSpellEntity { HeroId = warrior.Id, SpellId = slash.Id },
                new HeroSpellEntity { HeroId = warrior.Id, SpellId = shieldBash.Id },
                new HeroSpellEntity { HeroId = mage.Id, SpellId = fireball.Id },
                new HeroSpellEntity { HeroId = mage.Id, SpellId = frostBolt.Id }
            );
            db.SaveChanges();
        }
        catch (SqlException exception)
        {
            LastInitializationError = $"Connexion SQL impossible: {exception.Message}";
        }
        catch (Exception exception)
        {
            LastInitializationError = $"Initialisation BDD impossible: {exception.Message}";
        }
    }
}
