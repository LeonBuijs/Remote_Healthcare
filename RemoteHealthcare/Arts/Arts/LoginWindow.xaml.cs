using System.Windows;
using System.Windows.Threading;

namespace Arts;

public partial class LoginWindow : Window, ILoginCallback
{
    public NetworkProcessor networkProcessor { get; set; }

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
        
        //Zet een callback voor het antwoord op de login.
        networkProcessor.SetLoginCallback(this);
        
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
        Console.WriteLine($"OnLogin called with {response}");
        if (response == "0")
        {
            string title = "Login unsuccessfull";
            string content = "Your username and/or password is incorrect, please try again. psst (password is incorrect)";
            MessageBox.Show(content, title);                 
            Console.WriteLine("Unknown user or password!");
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
}