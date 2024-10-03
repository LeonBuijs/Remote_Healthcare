using System.Windows;
using System.Windows.Controls;      

namespace Arts;

public partial class ClientListWindow : Window
{
    public ClientListWindow()
    {
        InitializeComponent();
        // loginWindow.Hide();
        
    }

    /**
     * Listener op de lijst met clients.
     * Als er op een client geklikt wordt, zal deze methode een nieuw window openen.
     */
    private void ChosenClient(object sender, SelectionChangedEventArgs e)
    {
        if (ItemList.SelectedItem is ListBoxItem client)
        {
            ClientWindow Clientwindow = new ClientWindow();
            Console.WriteLine(client.Content);
            MessageBox.Show($"Je hebt {client.Content} geselecteerd.");        }
    }
}