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
        ClientListWindow mainWindow = new ClientListWindow();
        mainWindow.Show();
        this.Close();
        
    }

    private void TextChanged(object sender, TextChangedEventArgs e)
    {
        
    }
}