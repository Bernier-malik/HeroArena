using System.Collections.ObjectModel;

namespace HeroArena.ViewModels;

public sealed class SpellVMX
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Damage { get; init; }
    public string Description { get; init; } = string.Empty;
    public string HeroName { get; init; } = string.Empty;
}

public sealed class HeroVMX : ViewModelBase
{
    private int _currentHealth;

    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Avatar { get; init; } = "🧙";
    public int BaseHealth { get; init; }
    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            if (SetProperty(ref _currentHealth, value))
            {
                RaisePropertyChanged(nameof(HealthRatio));
            }
        }
    }

    public double HealthRatio => BaseHealth <= 0 ? 0 : (double)CurrentHealth / BaseHealth;
    public ObservableCollection<SpellVMX> Spells { get; } = [];
}
