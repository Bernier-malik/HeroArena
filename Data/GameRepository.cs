using System.Collections.Generic;
using System.Linq;
using HeroArena.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HeroArena.Data;

public sealed class GameRepository
{
    public List<HeroVMX> LoadHeroes()
    {
        using var db = new HeroArenaDbContext();
        var heroes = db.Heroes
            .Include(h => h.HeroSpells)
            .ThenInclude(hs => hs.Spell)
            .OrderBy(h => h.Name)
            .ToList();

        return heroes.Select(ToHeroVmx).ToList();
    }

    public List<SpellVMX> LoadSpells(string? heroNameFilter = null)
    {
        using var db = new HeroArenaDbContext();
        var query = db.HeroSpells
            .Include(hs => hs.Hero)
            .Include(hs => hs.Spell)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(heroNameFilter) && heroNameFilter != "Tous")
        {
            query = query.Where(hs => hs.Hero.Name == heroNameFilter);
        }

        return query
            .OrderBy(hs => hs.Hero.Name)
            .ThenBy(hs => hs.Spell.Name)
            .Select(hs => new SpellVMX
            {
                Id = hs.SpellId,
                Name = hs.Spell.Name,
                Damage = hs.Spell.Damage,
                Description = hs.Spell.Description ?? string.Empty,
                HeroName = hs.Hero.Name
            })
            .ToList();
    }

    private static HeroVMX ToHeroVmx(HeroEntity hero)
    {
        var vm = new HeroVMX
        {
            Id = hero.Id,
            Name = hero.Name,
            Avatar = GetAvatar(hero.Name),
            BaseHealth = hero.Health,
            CurrentHealth = hero.Health
        };

        foreach (var heroSpell in hero.HeroSpells.OrderBy(hs => hs.Spell.Name))
        {
            vm.Spells.Add(new SpellVMX
            {
                Id = heroSpell.SpellId,
                Name = heroSpell.Spell.Name,
                Damage = heroSpell.Spell.Damage,
                Description = heroSpell.Spell.Description ?? string.Empty,
                HeroName = hero.Name
            });
        }

        return vm;
    }

    private static string GetAvatar(string heroName)
    {
        return heroName switch
        {
            "Krogar l'Invincible" => "🛡️",
            "Lyra la Mage Noire" => "🧙",
            "Silas l'Ombre" => "🗡️",
            _ => "⚔️"
        };
    }
}
