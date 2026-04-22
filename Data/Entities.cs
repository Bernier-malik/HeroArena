using System.Collections.Generic;

namespace HeroArena.Data;

public sealed class LoginEntity
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public ICollection<PlayerEntity> Players { get; set; } = new List<PlayerEntity>();
}

public sealed class PlayerEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? LoginId { get; set; }
    public LoginEntity? Login { get; set; }
    public ICollection<PlayerHeroEntity> PlayerHeroes { get; set; } = new List<PlayerHeroEntity>();
}

public sealed class HeroEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Health { get; set; }
    public string? ImageUrl { get; set; }
    public ICollection<PlayerHeroEntity> PlayerHeroes { get; set; } = new List<PlayerHeroEntity>();
    public ICollection<HeroSpellEntity> HeroSpells { get; set; } = new List<HeroSpellEntity>();
}

public sealed class SpellEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Damage { get; set; }
    public string? Description { get; set; }
    public ICollection<HeroSpellEntity> HeroSpells { get; set; } = new List<HeroSpellEntity>();
}

public sealed class PlayerHeroEntity
{
    public int PlayerId { get; set; }
    public PlayerEntity Player { get; set; } = null!;
    public int HeroId { get; set; }
    public HeroEntity Hero { get; set; } = null!;
}

public sealed class HeroSpellEntity
{
    public int HeroId { get; set; }
    public HeroEntity Hero { get; set; } = null!;
    public int SpellId { get; set; }
    public SpellEntity Spell { get; set; } = null!;
}
