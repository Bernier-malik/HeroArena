# HeroArena

Application C# de combat tour par tour dans une arene medievale. Trois champions legendaires s'affrontent: Krogar le guerrier de legende, Lyra la puissante mage, et Silas l'assassin des ombres.

Developpe avec Avalonia (framework cross-platform MVVM), connecte a SQL Server via Entity Framework Core.

## Setup & Prerequis

- .NET SDK 10+
- SQL Server 2019+ ou SQL Server Express (local)
- Base de donnees nommee `ExerciceHero`

## Initialisation BD (premiere fois)

1. Executer le script `database-init.sql` dans SSMS
2. Lancer l'app une premiere fois
   - Un seeding automatique ajoute 3 champions + 12 spells
   - Compte test: `admin` / `admin123`

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Your_password123" -e "MSSQL_PID=Developer" -p 1433:1433 --name heroarena-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

## Connexion Base de Donnees

Par ordre de priorite:
1. Variable d'env: `HEROARENA_DB_CONNECTION`
2. Fichier `db-connection.txt` (sauvegardi par l'app via Settings)
3. Fallback hardcodé dans `Data/DatabaseSettings.cs`

C'est utile pour tester sur des machines differentes.

## Ce qui marche

### Login & Comptes
- ✅ Inscription et connexion avec hash SHA256 (Base64)
- ✅ Validation en base via EF Core
- ✅ Gestion d'erreurs correcte

### Gestion Heros
- ✅ Liste tous les 3 champions dispo
- ✅ Affichage detail: stats, HP, spells associés
- ✅ Interface fluide pour choisir son perso

### Grimoire (Spells)
- ✅ Voir tous les 12 spells du jeu
- ✅ Details complets (nom, degats, description)
- ✅ Filtre intelligent par hero

### Combat (Le plus fun)
- ✅ Combat tour par tour joueur vs ennemi
- ✅ Barres HP visibles pour les deux combattants
- ✅ L'ennemi reingenere (+10% HP, +5% degats pour augmenter difficulte)
- ✅ Messages perso quand un hero meurt (dependant du personnage)
- ✅ Bouton "Prochain combat" pour relancer
- ✅ Compteur de victoires/score

## Petits details techniques

- **MVVM:** Respecté avec suffixe `VMX` sur tous les ViewModels
- **DebugFlag:** Propriete inutilisée presentes dans MainVMX (requis par specs)
- **HP_MAX_WHEN_DIE:** Constante = 12 dans le code MainVMX.cs
- **Ressources:** Styles separées dans `Styles/ThemeResources.axaml`
- **EF Core:** Create + Read implémentées (Logins, Heroes, Spells avec relations)

## A savoir

- Chaque hero a **exactement 4 spells**
- Les spells ont des degats differents (Krogar plus fort, Silas + rapide)
- L'arene est un peu brutale avec les boosts d'ennemi... cest pas evidente!
- Je n'ai pas modifie le schema SQL initial

## Packages utilises

- `Avalonia` 12.0.1
- `Microsoft.EntityFrameworkCore` 8.0.4
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.4
