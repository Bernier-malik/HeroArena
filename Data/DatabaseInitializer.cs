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
                Name = "Malik",
                Login = login
            };

            // Trois champions legendaires de l'arene
            var krogar = new HeroEntity { Name = "Krogar l'Invincible", Health = 130 };
            var lyra = new HeroEntity { Name = "Lyra la Mage Noire", Health = 85 };
            var silas = new HeroEntity { Name = "Silas l'Ombre", Health = 105 };

            // Techniques du guerrier Krogar - puissant mais lent
            var coupTonnerre = new SpellEntity { Name = "Coup de Tonnerre", Damage = 22, Description = "Une frappe colossale qui secoue la terre." };
            var rueeSauvage = new SpellEntity { Name = "Ruee Sauvage", Damage = 16, Description = "Charge furieuse en avant." };
            var bouclierPierre = new SpellEntity { Name = "Bouclier de Pierre", Damage = 14, Description = "Protege et contre-attaque." };
            var trancheeFinale = new SpellEntity { Name = "Tranchee Finale", Damage = 28, Description = "Le coup qui decider d'une bataille." };

            // Sorts de Lyra - equilibree entre puissance et vitesse
            var eclairMagique = new SpellEntity { Name = "Eclair Magique", Damage = 26, Description = "Un rayon d'energie pure jaillit du ciel." };
            var tempeteGivre = new SpellEntity { Name = "Tempete de Givre", Damage = 19, Description = "Ralentit l'ennemi avec des cristaux glacés." };
            var chainesEtherees = new SpellEntity { Name = "Chaines Etherees", Damage = 23, Description = "Des liens magiques entravant l'adversaire." };
            var explosionStelaire = new SpellEntity { Name = "Explosion Stellaire", Damage = 31, Description = "Concentre toute la puissance des etoiles en un point." };

            // Techniques sournoise de Silas - rapide et letale
            var estocAssassin = new SpellEntity { Name = "Estoc Assassin", Damage = 18, Description = "Coup precis au point faible." };
            var poisonVirulent = new SpellEntity { Name = "Poison Virulent", Damage = 15, Description = "Lame trempee dans un venin ancien." };
            var evanouissement = new SpellEntity { Name = "Evanouissement", Damage = 13, Description = "Disprait dans les ombres." };
            var frappeFatale = new SpellEntity { Name = "Frappe Fatale", Damage = 32, Description = "Une attaque qui doit ecraser lors du premier coup." };

            db.Players.Add(player);
            db.Heroes.AddRange(krogar, lyra, silas);
            db.Spells.AddRange(
                coupTonnerre, rueeSauvage, bouclierPierre, trancheeFinale,
                eclairMagique, tempeteGivre, chainesEtherees, explosionStelaire,
                estocAssassin, poisonVirulent, evanouissement, frappeFatale);
            db.SaveChanges();

            db.PlayerHeroes.Add(new PlayerHeroEntity { PlayerId = player.Id, HeroId = krogar.Id });
            db.HeroSpells.AddRange(
                new HeroSpellEntity { HeroId = krogar.Id, SpellId = coupTonnerre.Id },
                new HeroSpellEntity { HeroId = krogar.Id, SpellId = rueeSauvage.Id },
                new HeroSpellEntity { HeroId = krogar.Id, SpellId = bouclierPierre.Id },
                new HeroSpellEntity { HeroId = krogar.Id, SpellId = trancheeFinale.Id },
                new HeroSpellEntity { HeroId = lyra.Id, SpellId = eclairMagique.Id },
                new HeroSpellEntity { HeroId = lyra.Id, SpellId = tempeteGivre.Id },
                new HeroSpellEntity { HeroId = lyra.Id, SpellId = chainesEtherees.Id },
                new HeroSpellEntity { HeroId = lyra.Id, SpellId = explosionStelaire.Id },
                new HeroSpellEntity { HeroId = silas.Id, SpellId = estocAssassin.Id },
                new HeroSpellEntity { HeroId = silas.Id, SpellId = poisonVirulent.Id },
                new HeroSpellEntity { HeroId = silas.Id, SpellId = evanouissement.Id },
                new HeroSpellEntity { HeroId = silas.Id, SpellId = frappeFatale.Id }
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
