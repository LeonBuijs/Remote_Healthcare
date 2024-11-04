using System.Windows;

namespace Arts;

/**
 * <summary>
 * Default klasse voor het opstarten van de applicatie
 * Start de loginwindow.
 * </summary>
 */
public partial class Program : Application
{
    /**
     * <summary>
     * Methode om een aantal verrichtingen te maken voordat de app is opgestart
     * Laadt het login window in.
     * </summary>
     */
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        LoginWindowWindow loginWindowWindow = new LoginWindowWindow();
        loginWindowWindow.Show();
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