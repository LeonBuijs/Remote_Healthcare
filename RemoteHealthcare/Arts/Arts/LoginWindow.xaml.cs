using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Controls;

namespace Arts;

public partial class LoginWindow : Window
{
    
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void OnLoginClick(object sender, RoutedEventArgs e)
    {
        string username = UsernameBox.Text;
        string password = PasswordBox.Password;
        Console.WriteLine($"Username: {username}\nPassword: {password}");
        
        // Program.TryLogin(username, password);
    }

}