using System;
using System.Linq;
using System.Windows.Input;
using HeroArena.Data;
using Microsoft.Data.SqlClient;

namespace HeroArena.ViewModels;

public sealed class LoginVMX : ViewModelBase
{
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                RaisePropertyChanged(nameof(HasError));
            }
        }
    }
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }
    public Action? LoginSucceeded { get; set; }

    public LoginVMX()
    {
        LoginCommand = new RelayCommand(Login);
        RegisterCommand = new RelayCommand(Register);
    }

    private void Login()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Renseigne le nom d'utilisateur et le mot de passe.";
            return;
        }

        try
        {
            using var db = new HeroArenaDbContext();
            var hashedPassword = PasswordHasher.ToBase64Sha256(Password);
            var canLogin = db.Logins.Any(l => l.Username == Username.Trim() && l.PasswordHash == hashedPassword);
            if (!canLogin)
            {
                ErrorMessage = "Identifiants invalides.";
                return;
            }

            ErrorMessage = string.Empty;
            LoginSucceeded?.Invoke();
        }
        catch (SqlException)
        {
            ErrorMessage = "Connexion SQL impossible. Verifie le serveur et la chaine de connexion.";
        }
        catch (Exception)
        {
            ErrorMessage = "Erreur inattendue pendant la connexion.";
        }
    }

    private void Register()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Renseigne un nom et un mot de passe pour creer un compte.";
            return;
        }

        try
        {
            using var db = new HeroArenaDbContext();
            var normalizedUsername = Username.Trim();
            if (db.Logins.Any(l => l.Username == normalizedUsername))
            {
                ErrorMessage = "Ce nom d'utilisateur existe deja.";
                return;
            }

            var login = new LoginEntity
            {
                Username = normalizedUsername,
                PasswordHash = PasswordHasher.ToBase64Sha256(Password)
            };

            db.Logins.Add(login);
            db.SaveChanges();
            ErrorMessage = "Compte cree. Tu peux te connecter.";
        }
        catch (SqlException)
        {
            ErrorMessage = "Connexion SQL impossible. Verifie le serveur et la chaine de connexion.";
        }
        catch (Exception)
        {
            ErrorMessage = "Erreur inattendue pendant la creation du compte.";
        }
    }
}
