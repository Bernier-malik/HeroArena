using Avalonia.Controls;
using Avalonia.Interactivity;
using HeroArena.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace HeroArena;

public partial class Login : Window
{
    private readonly TextBox? _usernameTextBox;
    private readonly TextBox? _passwordTextBox;
    private readonly TextBlock? _errorText;

    public Login()
    {
        InitializeComponent();
        _usernameTextBox = this.FindControl<TextBox>("UsernameTextBox");
        _passwordTextBox = this.FindControl<TextBox>("PasswordTextBox");
        _errorText = this.FindControl<TextBlock>("ErrorText");
    }

    private void GoToMain_Click(object? sender, RoutedEventArgs e)
    {
        var username = _usernameTextBox?.Text?.Trim();
        var password = _passwordTextBox?.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            if (_errorText is not null)
            {
                _errorText.Text = "Merci de remplir le nom d'utilisateur et le mot de passe.";
                _errorText.IsVisible = true;
            }
            return;
        }

        try
        {
            var hashedPassword = PasswordHasher.ToBase64Sha256(password);
            using var db = new HeroArenaDbContext();
            var canLogin = db.Logins.Any(l => l.Username == username && l.PasswordHash == hashedPassword);
            if (!canLogin)
            {
                if (_errorText is not null)
                {
                    _errorText.Text = "Identifiants invalides.";
                    _errorText.IsVisible = true;
                }
                return;
            }
        }
        catch (SqlException)
        {
            if (_errorText is not null)
            {
                _errorText.Text = "Connexion a la base impossible. Verifie SQL Server et la chaine HEROARENA_DB_CONNECTION.";
                _errorText.IsVisible = true;
            }
            return;
        }
        catch
        {
            if (_errorText is not null)
            {
                _errorText.Text = "Une erreur est survenue pendant la connexion.";
                _errorText.IsVisible = true;
            }
            return;
        }

        if (_errorText is not null)
        {
            _errorText.IsVisible = false;
        }

        var mainWindow = new Main();
        mainWindow.Show();
        Close();
    }
}

public partial class Main : Window
{
    public Main()
    {
        InitializeComponent();
    }
}