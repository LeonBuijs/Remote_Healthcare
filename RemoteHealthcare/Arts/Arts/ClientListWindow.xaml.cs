using System.Windows;
using System.Windows.Controls;      

namespace Arts;

public partial class ClientListWindow : Window
{
    private NetworkProcessor networkProcessor;
    public ClientListWindow(NetworkProcessor networkProcessor)
    {
        InitializeComponent();
        this.networkProcessor = networkProcessor;
    }

    /**
     * Listener op de lijst met clients.
     * Als er op een client geklikt wordt, zal deze methode een nieuw window openen.
     */
    private void ChosenClient(object sender, SelectionChangedEventArgs e)
    {
        if (ItemList.SelectedItem is ListBoxItem client)
        {
            //Get chosen clientId
            string? clientId = client.Content.ToString();
            
            MessageBox.Show($"Je hebt {clientId} geselecteerd.");
            
            ClientWindow clientWindow = new ClientWindow(clientId, networkProcessor);
            clientWindow.Show();
            Console.WriteLine(client.Content);
        }
    }

    private void RefreshClients(object sender, RoutedEventArgs routedEventArgs)
    {
        List<string> newClients = networkProcessor.GetClientList();
        ItemList.Items.Clear();
        newClients.ForEach(value => ItemList.Items.Add(value));
    }

    private void MakeClient(object sender, RoutedEventArgs routedEventArgs)
    {
        networkProcessor.MakeClient(ClientName.Text + " " + ClientdateOfBirth.Text);
    }
}