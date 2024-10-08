using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Arts;

public partial class Program : Application
{
    private static TcpClient artsClient;
    private static NetworkStream artsStream;
    private static DataSender artsSender;
    private static byte[] artsBuffer = new byte[128];
    private static string totalBuffer;
    private static List<IDataUpdateCallback> dataUpdateCallbacks = new List<IDataUpdateCallback>();
    private static List<string> clients = new List<string>();
    
    //todo ophalen van GUI echter met testen hardcoded
    private static string username = "Jan12";
    private static string password = "incorrect";


    /**
     * Methode om een aantal verrichtingen te maken voordat de app is opgestart
     * Voor nu: Connectie met de server maken.
     */
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        artsClient = new TcpClient();
        //todo verander de host en poortnummer
        try
        {
            await artsClient.ConnectAsync("127.0.0.1", 7777);
            //won't come here until it has connection.
            new Thread(OnConnect).Start(); 
            artsSender = new DataSender(artsClient.GetStream());
        }
        catch (SocketException exception)
        {
            //todo try to reconnect/give pop up
            Console.WriteLine("Can't connect to server");
        }
        
    }

    /**
     * Methode die het afsluiten van de applicatie afhandelt
     * Voor de zekerheid sluiten we alle streams en client
     */
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        artsClient?.Close();
        artsStream?.Close();
        Environment.Exit(0);
    }


    /**
     * Methode om het zoeken naar verbinding te stoppen
     * Gebeurt bij gevonden verbinding en start een listener methode
     */
    private static void OnConnect()
    {
        Console.WriteLine("Verbonden!");
        artsStream = artsClient.GetStream();
        artsStream.BeginRead(artsBuffer, 0, artsBuffer.Length, new AsyncCallback(OnRead), null);

        artsSender = new DataSender(artsStream);
        // artsSender.SendLogin(username, password);
    }

    /**
     * Het uitlezen van data die het vervolgens naar een methode stuurt om te verwerken
     */
    private static void OnRead(IAsyncResult ar)
    {
        int receivedBytes = artsStream.EndRead(ar);
        string receivedText = Encoding.ASCII.GetString(artsBuffer, 0, receivedBytes);
        totalBuffer += receivedText;

        //todo: pakket opsplitsen
        string[] packetSplit = receivedText.Split(" ");
        HandleData(packetSplit);

        artsStream.BeginRead(artsBuffer, 0, artsBuffer.Length, new AsyncCallback(OnRead), null);
    }

    /**
     * Methode die gebruik maakt van ons protocol om de inkomende data af te handelen
     * 0: inloggegevens bevestigen
     * 1: realtime data verwerken
     * 2: lijst met actieve sessies tonen
     * 3: lijst met opgeslagen data per sessie
     */
    private static void HandleData(string[] packetData)
    {
        int packetPage = int.Parse(packetData[0]);
        switch (packetPage)
        {
            case 0:
                string argument = packetData[1];
                if (argument == "0")
                {
                    string title = "Login unsuccesfull";
                    string content = "Your username and/or password is incorrect, please try again. psst (paasword is incorrect)";
                    MessageBox.Show(content, title);                 
                    Console.WriteLine("Unknown user or password!");
                    //todo afhandelen geweigerd
                }
                else if (argument == "1")
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Console.WriteLine("Logged in!");
                        ClientListWindow clientListWindow = new ClientListWindow();
                        clientListWindow.Show();
                    });
                    
                    //todo afhandelen geaccepteerd
                }
                else
                {
                    Console.WriteLine("Something went wrong!");
                    //todo afhandelen overige fouten
                }

                break;
            case 1:
                //todo koppelen met GUI
                // string speed = packetData[1];
                // string distance = packetData[2];
                // string power = packetData[3];
                // string RPM = packetData[4];
                // string heartbeat = packetData[5];
                
                //todo make parameters
                string clientId = packetData[1].Replace(";", " ");
                string data = packetData[2].Replace(";", " ");
                dataUpdateCallbacks.ForEach(callbackMember => callbackMember.UpdateData(clientId, data));
                // Console.WriteLine($"received packet page: {packetPage} with {speed} speed, {distance} distance, " +
                //                   $"{power} power, {RPM} RPM, {heartbeat} heartbeat");
                break;
            case 2:
                clients.Add(packetData[1].Replace(";", " "));
                break;
            case 3:
                break;
            default:
                Console.WriteLine("Unknown Packet Page");
                //todo better error handling
                break;
        }
    }
    
    public static void TryLogin(string username, string password){
        artsSender.SendLogin(username, password);
    }

    public static void AddCallbackMember(IDataUpdateCallback callbackMember)
    {
        dataUpdateCallbacks.Add(callbackMember);
    }

    public static List<string> GetClientList()
    {
        return clients;
    }


}