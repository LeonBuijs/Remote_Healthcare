using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;      

namespace Arts;

public partial class ClientListWindow : Window, IListWindowCallback
{
    private NetworkProcessor networkProcessor;
    private ObservableCollection<string> fileNames;
    public ClientListWindow(NetworkProcessor networkProcessor)
    {
        InitializeComponent();
        this.networkProcessor = networkProcessor;
        fileNames = new ObservableCollection<string>();
        ItemList.ItemsSource = fileNames;
        networkProcessor.ListWindowCallback = this;
        Refresh();

    }

    /**
     * Listener op de lijst met clients.
     * Als er op een client geklikt wordt, zal deze methode een nieuw window openen.
     */
    private void ChosenClient(object sender, SelectionChangedEventArgs e)
    {
        if (ItemList.SelectedItem is string client)
        {
            //Get chosen clientId
            string? clientId = client;
            
            MessageBox.Show($"Je hebt {clientId} geselecteerd.");
            
            ClientWindow clientWindow = new ClientWindow(clientId, networkProcessor);
            clientWindow.Show();
            Console.WriteLine(client);
        }
    }

    private void RefreshClientsPressed(object sender, RoutedEventArgs routedEventArgs)
    {
        Refresh();
    }

    private void MakeClientPressed(object sender, RoutedEventArgs routedEventArgs)
    {
        networkProcessor.MakeClient(ClientName.Text + " " + ClientdateOfBirth.Text);
    }

    private void Refresh()
    {
        fileNames.Clear();
        networkProcessor.RefreshClientList();
    }

    /**
     * <summary>
     * Methode die een nieuwe client aan de lijst toevoegd.
     * Als de client al in de list zit zal hij returnen en niks toevoegen.
     * </summary>
     * <param name="clientId">De client naam en geboortedatum</param>
     */
    public void AddNewClient(string clientId)
    {
        Dispatcher.Invoke(() =>
        {
            if (fileNames.Contains(clientId))
                return;

            fileNames.Add(clientId);
        });
    }

    /**
     * <summary>
     * Methode die een client verwijdert van de lijst
     * Als de client niet bestaat zal hij returnen.
     * </summary>
     * <param name="clientId">De client naam en geboortedatum</param>
     */
    public void RemoveClient(string clientId)
    {
        Dispatcher.Invoke(() =>
        {
            if (!fileNames.Contains(clientId))
                return;
        
            fileNames.Remove(clientId);
        });
    }
}