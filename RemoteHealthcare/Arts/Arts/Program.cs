using System.Windows;

namespace Arts;

public partial class Program : Application
{
    /**
     * <summary>
     * Methode om een aantal verrichtingen te maken voordat de app is opgestart
     * Zet een connectie op met de server via networkProcessor
     * Laadt het login window in.
     * </summary>
     */
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        NetworkProcessor networkProcessor = new NetworkProcessor();
        
        LoginWindow loginWindow = new LoginWindow();
        loginWindow.networkProcessor = networkProcessor;
        loginWindow.Show();
    }

    /**
     * <summary>
     * Methode die het afsluiten van de applicatie afhandelt
     * De extra functie is dat wanneer je het venster sluit via het kruisje,
     * De code ook afsluit en niet alleen het GUI.
     * </summary>
     */
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        Environment.Exit(0);
    }


}