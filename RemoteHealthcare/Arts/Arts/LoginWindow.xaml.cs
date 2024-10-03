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

    private void ButtonClick(object sender, RoutedEventArgs e)
    {
        //todo methode maken die login checked
        if (false)
        {
            ClientListWindow mainWindow = new ClientListWindow();
            mainWindow.Show();
            this.Close();
        }
        
        
    }

    private void TextChanged(object sender, TextChangedEventArgs e)
    {
        
    }
}