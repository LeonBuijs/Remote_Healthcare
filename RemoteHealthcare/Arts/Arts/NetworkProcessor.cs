using System.Net.Sockets;
using System.Text;
using System.Windows;


namespace Arts;

public class NetworkProcessor
{
    private TcpClient artsClient;
    private NetworkStream artsStream;
    private DataSender artsSender;
    private byte[] artsBuffer = new byte[128];
    private string totalBuffer;

    private String ipAdress;
    
    public ILoginWindowCallback LoginWindowCallback { set; get; }
    public IListWindowCallback ListWindowCallback { set; get; }
    
    private List<IDataUpdateCallback> dataUpdateCallbacks = new List<IDataUpdateCallback>();
    private List<string> clientsWhoRecieveData = new List<string>();
    private bool isAskingData = false;

    public NetworkProcessor(string ipAdres)
    {
        artsClient = new TcpClient();
        ipAdress = ipAdres;
        ConnectToServer();
    }
    
    public void ConnectToServer()
    {
        //todo verander de host en poortnummer
        try
        {
            artsClient.Connect(ipAdress, 7777);
            //won't come here until it has connection.
            new Thread(OnConnect).Start(); 
            artsSender = new DataSender(artsClient.GetStream());
        }
        catch (SocketException exception)
        {
            //Notificeert het inlog scherm dat de connectie is gefaald.
            LoginWindowCallback.ConnectionFailed();
        }
    }
    
    /**
     * Methode om het zoeken naar verbinding te stoppen
     * Gebeurt bij gevonden verbinding en start een listener methode
     */
    private void OnConnect()
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
    private void OnRead(IAsyncResult ar)
    {
        int receivedBytes = artsStream.EndRead(ar);
        string receivedText = Encoding.ASCII.GetString(artsBuffer, 0, receivedBytes);
        totalBuffer += receivedText;
        
        string[] packetSplit = receivedText.Split(" ");
        HandleData(packetSplit);

        artsStream.BeginRead(artsBuffer, 0, artsBuffer.Length, new AsyncCallback(OnRead), null);
    }

    /**
     * Methode die gebruik maakt van ons protocol om de inkomende data af te handelen
     * 0: inloggegevens bevestigen, gebeurd verder in de inlog window.
     * 1: realtime data verwerken
     * 2: lijst met actieve sessies tonen
     * 3: lijst met opgeslagen data per sessie
     */
    private void HandleData(string[] packetData)
    {
        int packetPage = int.Parse(packetData[0]);
        switch (packetPage)
        {
            case 0:
                string argument = packetData[1];
                Console.WriteLine($"Got login answer with argument {argument}");
                LoginWindowCallback.OnLogin(argument);
                break;
            case 1:
                string clientId = packetData[1].Replace(";", " ");
                string data = packetData[2].Replace(";", " ");
                Console.WriteLine($"Got client \"{clientId}\" with data \"{data}\"");
                
                dataUpdateCallbacks.ForEach(callbackMember => callbackMember.UpdateData(clientId, data));
                break;
            case 2:
                string newClientId = packetData[1].Replace(";", " ");
                Console.WriteLine($"Kreeg clientId {newClientId}");
                ListWindowCallback.AddNewClient(newClientId);
                break;
            case 3:
                break;
            default:
                Console.WriteLine("Unknown Packet Page");
                //todo better error handling
                break;
        }
    }

    public void AddActiveClient(string clientId)
    {
        Console.WriteLine($"adding {clientId} to active clients");
        lock (clientsWhoRecieveData)
        {
            Console.WriteLine($"{clientId} added to clients list!");
            if (!clientsWhoRecieveData.Contains(clientId))
            {
                clientsWhoRecieveData.Add(clientId);
            }    
        }
        
        if (isAskingData)
        {
            return;
        }
        
        isAskingData = true;
        new Thread(() =>
        {
            //Bij het starten zal er altijd 1 waarde zijn in de lijst.
            int count = 1;
            
            while (count > 0)
            {
                lock (clientsWhoRecieveData)
                {
                    clientsWhoRecieveData.ForEach(GetRealtimeData);
                    count = clientsWhoRecieveData.Count;
                }
                Thread.Sleep(500);
            }
            isAskingData = false;
        }).Start();

    }

    public bool IsConnected()
    {
        return artsClient.Connected;
    }
    
    public void TryLogin(string username, string password){
        artsSender.SendLogin(username, password);
    }
    
    public void AddCallbackMember(IDataUpdateCallback callbackMember)
    {
        dataUpdateCallbacks.Add(callbackMember);
    }

    public void MakeClient(string clientInfo)
    {
        artsSender.MakeClient(clientInfo);
    }

    public void GetRealtimeData(string clientInfo)
    {
        artsSender.ChosenClient(clientInfo);
    }

    public void RefreshClientList()
    {
        artsSender.GetClients();
    }

    public void StartClientSessie(string clientInfo)
    {
        Console.WriteLine($"telling server {clientInfo} started");
        artsSender.StartSession(clientInfo);
    }

    public void StopClientSessie(string clientInfo)
    {
        lock (clientsWhoRecieveData)
        {
            clientsWhoRecieveData.Remove(clientInfo);
        }
        artsSender.StopSession(clientInfo);
    }

    public void EmergencyStopClientSessie(string clientInfo)
    {
        artsSender.EmergencyStopSession(clientInfo);
    }

    public void SendConfigs(string session, string resistance)
    {
        artsSender.SendBikeConfigs(session, resistance);
    }
    
    public void SendMessage(string clientInfo, string messege)
    {
        artsSender.SendMessageToSession(clientInfo, messege);
    }

    public void SendMessageToAll(string message)
    {
        artsSender.SendMessageToAllSessions(message);
    }
}