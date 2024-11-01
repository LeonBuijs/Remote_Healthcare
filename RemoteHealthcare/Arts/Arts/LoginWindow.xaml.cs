using System.Windows;
using System.Windows.Threading;

namespace Arts;

public partial class LoginWindowWindow : Window, ILoginWindowCallback
{
    private NetworkProcessor? networkProcessor;

    public LoginWindowWindow()
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
        Console.WriteLine("\nOnLoginClick start");
        string ipAddress = IpAdressBox.Text.Trim();
        
        //Controleert of er überhaupt een ip adres is ingevoerd
        if (string.IsNullOrEmpty(ipAddress))
        {
            ShowMissingIpAddressMessage();
            return;
        }
        
        //Controleert of het een geldig ip adres is (qua format)
        if (!CheckIpAddress(ipAddress))
        {
            ShowInvalidIpAddressMessage();
            return;
        }
                
        if (networkProcessor == null || !networkProcessor.IsConnected())
        {
            networkProcessor = new NetworkProcessor(ipAddress, this);
        }
        
        string username = UsernameBox.Text;
        string password = PasswordBox.Password;
        
        //Stuur aanzoek voor inloggen
        networkProcessor.TryLogin(username, password);
    }

    
    /**
     * <summary>
     * Methode die wordt die het antwoord op de login afhandelt.
     * 0: De login poging is afgewezen
     * 1: De login poging is geaccepteerd, ga door naar het volgende venster
     * De laatste else vangt onbekende errors af.
     * </summary>
     * <param name="response">De logincode vanuit de server</param>
     */
    public void OnLogin(string response)
    {
        if (response == "0")
        {
            string title = "Login unsuccessfull";
            string content = "Your username and/or password is incorrect, please try again. psst (password is incorrect)";
            MessageBox.Show(content, title);                 
        }
        else if (response == "1")
        {
            Dispatcher.Invoke(() =>
                {
                    Console.WriteLine("Logged in!");
                    ClientListWindow clientListWindow = new ClientListWindow(networkProcessor);
                    clientListWindow.Show();
                    Close();
                }
            );

        }
        else
        {
            string title = "Unknown error";
            string content = "Something went wrong, please try again.";
            MessageBox.Show(content, title);
            Console.WriteLine("Something went wrong!");
        }
    }
    
    /**
     * <summary>
     * Methode die een pop-up geeft wanneer de connectie gefaald is.
     * Deze pop-up vraagt of de dokter het opnieuw wil proberen te verbinden.
     * </summary>
     */
    public void ConnectionFailed()
    {
        ShowRetryConnectionMessage();
    }

    private bool CheckIpAddress(string ipAddress)
    {
        return System.Net.IPAddress.TryParse(ipAddress, out _);
    }

    #region MessageBoxes
    
    private void ShowRetryConnectionMessage()
    {
        string title = "ERROR 404";
        string content = "Connection failed, do you want to retry?";
        MessageBoxResult result = MessageBox.Show(content, title, 
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Yes)
        {
            if (networkProcessor == null)
            {
                MessageBox.Show("Can't access network, please check the ip-address");
                return;
            }
            networkProcessor.ConnectToServer();
        }
    }

    private void ShowMissingIpAddressMessage()
    {
        string title = "No IP address provided";
        string content = "Please fill in an IP address.";
        MessageBox.Show(content, title);
    }

    private void ShowInvalidIpAddressMessage()
    {
        string title = "INVALID IP ADDRESS";
        string content = "IP Address is not valid.";
        MessageBox.Show(content, title);
    }
    
    /**
     * <summary>
     * Eventuele methode voor een debugger/simulator
     * Deze zou weergegeven kunnen worden na een x aantal foute verbindings pogingen
     * </summary>
     */
    private void ShowDebugModeMessage()
    {
        string title = "Debug mode";
        string content = "Server seems unresponsive, do you want to enable debug mode?";
        MessageBoxResult result = MessageBox.Show(content, title, 
            MessageBoxButton.YesNo, MessageBoxImage.Error);
        if (result == MessageBoxResult.Yes)
        {
            OnLogin("1");
        } else
        {
            // Wanneer er eventueel simulatie wordt gemaakt voeg teller toe!!
            // amountOfFailedLogins = 0;
        }
    }
    #endregion
}