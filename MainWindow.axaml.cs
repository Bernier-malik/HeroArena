using Avalonia.Controls;
using HeroArena.ViewModels;

namespace HeroArena;

public partial class Login : Window
{
    public Login()
    {
        InitializeComponent();
        var vm = new LoginVMX();
        vm.LoginSucceeded = () =>
        {
            var mainWindow = new Main();
            mainWindow.Show();
            Close();
        };
        DataContext = vm;
    }
}

public partial class Main : Window
{
    public Main()
    {
        InitializeComponent();
        DataContext = new MainVMX();
    }
}