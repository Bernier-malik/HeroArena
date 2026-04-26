using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HeroArena.Data;

namespace HeroArena.ViewModels;

public sealed class MainVMX : ViewModelBase
{
    public const int HP_MAX_WHEN_DIE = 12;
    public bool DebugFlag { get; set; }

    private const int EnemyResponseDelayMs = 800;
    private readonly GameRepository _repository = new();
    private readonly Random _random = new();

    private HeroVMX? _selectedHero;
    private SpellVMX? _selectedSpell;
    private HeroVMX? _playerHero;
    private HeroVMX? _enemyHero;
    private SpellVMX? _selectedCombatSpell;
    private bool _isPlayerTurn = true;
    private string _combatLog = "Choisis un hero joueur pour commencer.";
    private int _score;
    private string _selectedHeroFilter = "Tous";
    private string _connectionString = DatabaseSettings.GetConnectionString();

    public ObservableCollection<HeroVMX> Heroes { get; } = [];
    public ObservableCollection<SpellVMX> Spells { get; } = [];
    public ObservableCollection<string> HeroFilters { get; } = [];

    public HeroVMX? SelectedHero
    {
        get => _selectedHero;
        set => SetProperty(ref _selectedHero, value);
    }

    public SpellVMX? SelectedSpell
    {
        get => _selectedSpell;
        set => SetProperty(ref _selectedSpell, value);
    }

    public HeroVMX? PlayerHero
    {
        get => _playerHero;
        set => SetProperty(ref _playerHero, value);
    }

    public HeroVMX? EnemyHero
    {
        get => _enemyHero;
        set => SetProperty(ref _enemyHero, value);
    }

    public SpellVMX? SelectedCombatSpell
    {
        get => _selectedCombatSpell;
        set => SetProperty(ref _selectedCombatSpell, value);
    }

    public bool IsPlayerTurn
    {
        get => _isPlayerTurn;
        set => SetProperty(ref _isPlayerTurn, value);
    }

    public string CombatLog
    {
        get => _combatLog;
        set => SetProperty(ref _combatLog, value);
    }

    public int Score
    {
        get => _score;
        set => SetProperty(ref _score, value);
    }

    public string SelectedHeroFilter
    {
        get => _selectedHeroFilter;
        set
        {
            if (SetProperty(ref _selectedHeroFilter, value))
            {
                LoadSpells();
            }
        }
    }

    public string ConnectionString
    {
        get => _connectionString;
        set => SetProperty(ref _connectionString, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand SelectHeroCommand { get; }
    public ICommand AttackCommand { get; }
    public ICommand NewFightCommand { get; }
    public ICommand SaveConnectionCommand { get; }

    public MainVMX()
    {
        RefreshCommand = new RelayCommand(LoadAll);
        SelectHeroCommand = new RelayCommand(SelectHeroForCombat);
        AttackCommand = new RelayCommand(Attack);
        NewFightCommand = new RelayCommand(StartNewFight);
        SaveConnectionCommand = new RelayCommand(SaveConnectionAndReload);

        LoadAll();
    }

    private void LoadAll()
    {
        Heroes.Clear();
        foreach (var hero in _repository.LoadHeroes())
        {
            Heroes.Add(hero);
        }

        HeroFilters.Clear();
        HeroFilters.Add("Tous");
        foreach (var heroName in Heroes.Select(h => h.Name))
        {
            HeroFilters.Add(heroName);
        }

        if (!HeroFilters.Contains(SelectedHeroFilter))
        {
            SelectedHeroFilter = "Tous";
        }

        LoadSpells();
    }

    private void LoadSpells()
    {
        Spells.Clear();
        foreach (var spell in _repository.LoadSpells(SelectedHeroFilter))
        {
            Spells.Add(spell);
        }
    }

    private void SelectHeroForCombat()
    {
        if (SelectedHero is null)
        {
            CombatLog = "Selectionne un hero dans l'onglet Heroes.";
            return;
        }

        PlayerHero = CloneHero(SelectedHero, 1.0, 1.0);
        EnemyHero = CloneHero(Heroes[_random.Next(Heroes.Count)], 1.10, 1.05);
        SelectedCombatSpell = PlayerHero.Spells.FirstOrDefault();
        IsPlayerTurn = true;
        CombatLog = $"Combat lance: {PlayerHero.Name} VS {EnemyHero.Name}. A toi de jouer.";
    }

    private async void Attack()
    {
        if (PlayerHero is null || EnemyHero is null)
        {
            CombatLog = "Lance d'abord un combat.";
            return;
        }

        if (!IsPlayerTurn)
        {
            CombatLog = "Attends la fin du tour ennemi.";
            return;
        }

        if (SelectedCombatSpell is null)
        {
            CombatLog = "Selectionne un sort avant d'attaquer.";
            return;
        }

        var spell = SelectedCombatSpell;
        EnemyHero.CurrentHealth = Math.Max(0, EnemyHero.CurrentHealth - spell.Damage);

        if (EnemyHero.CurrentHealth <= 0)
        {
            Score += 1;
            CombatLog = $"{PlayerHero.Name} gagne! {GetDeathMessage(EnemyHero.Name)} Score: {Score}";
            IsPlayerTurn = true;
            return;
        }

        IsPlayerTurn = false;
        CombatLog = $"{PlayerHero.Name} utilise {spell.Name} ({spell.Damage}). {EnemyHero.Name} prepare sa riposte...";
        await Task.Delay(EnemyResponseDelayMs);

        if (PlayerHero is null || EnemyHero is null || EnemyHero.CurrentHealth <= 0)
        {
            IsPlayerTurn = true;
            return;
        }

        var enemySpell = EnemyHero.Spells[_random.Next(EnemyHero.Spells.Count)];
        PlayerHero.CurrentHealth = Math.Max(0, PlayerHero.CurrentHealth - enemySpell.Damage);
        CombatLog = $"{PlayerHero.Name} utilise {spell.Name} ({spell.Damage}) | " +
                    $"{EnemyHero.Name} repond avec {enemySpell.Name} ({enemySpell.Damage}).";

        if (PlayerHero.CurrentHealth <= 0)
        {
            CombatLog += $" {GetDeathMessage(PlayerHero.Name)}";
            PlayerHero.CurrentHealth = HP_MAX_WHEN_DIE;
        }

        IsPlayerTurn = true;
    }

    private void StartNewFight()
    {
        if (Heroes.Count == 0)
        {
            CombatLog = "Aucun hero disponible.";
            return;
        }

        if (PlayerHero is null)
        {
            PlayerHero = CloneHero(Heroes[0], 1.0, 1.0);
        }

        EnemyHero = CloneHero(Heroes[_random.Next(Heroes.Count)], 1.10, 1.05);
        SelectedCombatSpell = PlayerHero.Spells.FirstOrDefault();
        IsPlayerTurn = true;
        CombatLog = $"Nouvel ennemi: {EnemyHero.Name} avec stats ameliorees. A toi de jouer.";
    }

    private void SaveConnectionAndReload()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            CombatLog = "La chaine de connexion est vide.";
            return;
        }

        DatabaseSettings.SaveConnectionString(ConnectionString);
        CombatLog = "Chaine sauvegardee. Rechargement des donnees...";
        LoadAll();
    }

    private HeroVMX CloneHero(HeroVMX source, double hpMultiplier, double damageMultiplier)
    {
        var newHealth = (int)Math.Ceiling(source.BaseHealth * hpMultiplier);
        var clone = new HeroVMX
        {
            Id = source.Id,
            Name = source.Name,
            Avatar = source.Avatar,
            BaseHealth = newHealth,
            CurrentHealth = newHealth
        };

        foreach (var spell in source.Spells)
        {
            clone.Spells.Add(new SpellVMX
            {
                Id = spell.Id,
                Name = spell.Name,
                Damage = (int)Math.Ceiling(spell.Damage * damageMultiplier),
                Description = spell.Description,
                HeroName = spell.HeroName
            });
        }

        return clone;
    }

    private static string GetDeathMessage(string heroName)
    {
        return heroName switch
        {
            "Krogar l'Invincible" => "Krogar tombe, dommage, il n'a pa pris d'assurence vie",
            "Lyra la Mage Noire" => "Lyra disparait dans une explosion, ces origines l'ont tué",
            "Silas l'Ombre" => "Silas sort de l'ombre pour rester dans la tombe. Pas plus discrete que ca.",
            _ => $"{heroName} est vaincu. Pas mal comme duel!"
        };
    }
}