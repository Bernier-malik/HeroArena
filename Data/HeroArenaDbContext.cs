using Microsoft.EntityFrameworkCore;

namespace HeroArena.Data;

public sealed class HeroArenaDbContext : DbContext
{
    public DbSet<LoginEntity> Logins => Set<LoginEntity>();
    public DbSet<PlayerEntity> Players => Set<PlayerEntity>();
    public DbSet<HeroEntity> Heroes => Set<HeroEntity>();
    public DbSet<SpellEntity> Spells => Set<SpellEntity>();
    public DbSet<PlayerHeroEntity> PlayerHeroes => Set<PlayerHeroEntity>();
    public DbSet<HeroSpellEntity> HeroSpells => Set<HeroSpellEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(DatabaseSettings.GetConnectionString());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoginEntity>(entity =>
        {
            entity.ToTable("Login");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<PlayerEntity>(entity =>
        {
            entity.ToTable("Player");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.HasOne(e => e.Login)
                .WithMany(e => e.Players)
                .HasForeignKey(e => e.LoginId);
        });

        modelBuilder.Entity<HeroEntity>(entity =>
        {
            entity.ToTable("Hero");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Health).IsRequired();
            entity.Property(e => e.ImageUrl).HasColumnName("ImageURL").HasMaxLength(255);
        });

        modelBuilder.Entity<SpellEntity>(entity =>
        {
            entity.ToTable("Spell");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Damage).IsRequired();
            entity.Property(e => e.Description);
        });

        modelBuilder.Entity<PlayerHeroEntity>(entity =>
        {
            entity.ToTable("PlayerHero");
            entity.HasKey(e => new { e.PlayerId, e.HeroId });
            entity.HasOne(e => e.Player)
                .WithMany(e => e.PlayerHeroes)
                .HasForeignKey(e => e.PlayerId);
            entity.HasOne(e => e.Hero)
                .WithMany(e => e.PlayerHeroes)
                .HasForeignKey(e => e.HeroId);
        });

        modelBuilder.Entity<HeroSpellEntity>(entity =>
        {
            entity.ToTable("HeroSpell");
            entity.HasKey(e => new { e.HeroId, e.SpellId });
            entity.HasOne(e => e.Hero)
                .WithMany(e => e.HeroSpells)
                .HasForeignKey(e => e.HeroId);
            entity.HasOne(e => e.Spell)
                .WithMany(e => e.HeroSpells)
                .HasForeignKey(e => e.SpellId);
        });
    }
}
