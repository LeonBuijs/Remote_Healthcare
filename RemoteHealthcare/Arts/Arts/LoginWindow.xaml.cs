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

    /**
     * Methode die wordt getriggerd bij het klikken op de login knop
     * Parameters:
     * - UsernameBox: komt uit de X-AML file. Verwijst naar de username-textbox
     * - PasswordBox: komt uit de X-AML file. Verwijst naar de password-box
     * Deze kan je direct benaderen en hoef je niet eerst op te slaan
     */
    private void OnLoginClick(object sender, RoutedEventArgs e)
    {
        string username = UsernameBox.Text;
        string password = PasswordBox.Password;
        Console.WriteLine($"Username: {username}\nPassword: {password}");
        
        Program.TryLogin(username, password);
    }

}