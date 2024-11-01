using System.Net.Sockets;
using System.Text;


namespace Arts;

public class NetworkProcessor
{
    private TcpClient artsClient;
    private NetworkStream artsStream;
    private DataSender artsSender;
    private byte[] artsBuffer = new byte[128];
    private string totalBuffer;

    private string ipAdress;

    private readonly ILoginWindowCallback LoginWindowCallback;
    public IListWindowCallback ListWindowCallback { set; get; }

    private List<IDataUpdateCallback> dataUpdateCallbacks = new List<IDataUpdateCallback>();
    private List<string> clientsWhoRecieveData = new List<string>();
    private bool isAskingData = false;

    public NetworkProcessor(string ipAddress, ILoginWindowCallback loginWindowWindowCallback)
    {
        artsClient = new TcpClient();
        ipAdress = ipAddress;
        LoginWindowCallback = loginWindowWindowCallback;
        ConnectToServer();
    }

    public void ConnectToServer()
    {
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
            Console.WriteLine("loginCallback");
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

        string[] multipleDataReivedSplit = receivedText.Split('\n');
        foreach (var argument in multipleDataReivedSplit)
        {
            Console.WriteLine($"argument: {argument}");
            string[] argumentSplit = argument.Split(" ");
            if (argumentSplit.Length > 1)
            {
                Console.WriteLine("faka");
                HandleData(argumentSplit);   
            }
        }

        artsStream.BeginRead(artsBuffer, 0, artsBuffer.Length, new AsyncCallback(OnRead), null);
    }

    /**
     * Methode die gebruik maakt van ons protocol om de inkomende data af te handelen
     * 0: inloggegevens bevestigen, gebeurd verder in de inlog window.
     * 1: realtime data verwerken
     * 2: lijst met actieve sessies tonen
     * 3: lijst met opgeslagen data per sessie
     */
    private void HandleData(string[] argumentData)
    {
        if (argumentData.Length == 0)
        {
            return;
        }

        var packetPage = int.Parse(argumentData[0]);
        
        switch (packetPage)
        {
            case 0:
                string argument = argumentData[1];
                Console.WriteLine($"Got login answer with argument {argument}");
                LoginWindowCallback.OnLogin(argument);
                break;
            case 1:
                string clientId = $"{argumentData[1]} {argumentData[2]} {argumentData[3]}";

                string data =
                    $"{argumentData[4]} {argumentData[5]} {argumentData[6]} {argumentData[7]} {argumentData[8]} {argumentData[9]}";
                Console.WriteLine($"Got client \"{clientId}\" with data \"{data}\"");

                dataUpdateCallbacks.ForEach(callbackMember => callbackMember.UpdateData(clientId, data));
                break;
            case 2:
                string newclientId = $"{argumentData[1]} {argumentData[2]} {argumentData[3]}";

                Console.WriteLine($"Kreeg clientId {newclientId}");
                ListWindowCallback.AddNewClient(newclientId);
                break;
            case 3:
                string historyclientId = $"{argumentData[1]} {argumentData[2]} {argumentData[3]}";

                foreach (var clientWindow in dataUpdateCallbacks)
                {
                    if (clientWindow.GetClientinfo().Equals(historyclientId))
                    {
                        string date = argumentData[4];
                        // {date} {duration}(0) {averageSpeed}(1) {maxSpeed}(1) {averageHeartRate}(2) {maxHeartRate}(2) {distance}(3)
                        clientWindow.UpdateHistory(0,Int32.Parse(argumentData[5]), date);
                        clientWindow.UpdateHistory(1,Int32.Parse(argumentData[6]), date);
                        clientWindow.UpdateHistory(1,Int32.Parse(argumentData[7]), date, 1);
                        clientWindow.UpdateHistory(2,Int32.Parse(argumentData[8]), date);
                        clientWindow.UpdateHistory(2,Int32.Parse(argumentData[9]), date, 1);
                        clientWindow.UpdateHistory(3,Int32.Parse(argumentData[10]), date);
                        Console.WriteLine("1 line of hsitory added");
                    }
                }
                break;
            default:
                Console.WriteLine("Unknown Packet Page");
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

    public void TryLogin(string username, string password)
    {
        if (!IsConnected())
        {
            return;
        }

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

    private void GetRealtimeData(string clientInfo)
    {
        artsSender.ChosenClient(clientInfo);
    }

    public void RefreshClientList()
    {
        artsSender.GetClients();
    }

    public void StartClientSession(string clientInfo)
    {
        Console.WriteLine($"telling server {clientInfo} started");
        artsSender.StartSession(clientInfo);
    }

    public void StopClientSession(string clientInfo)
    {
        lock (clientsWhoRecieveData)
        {
            clientsWhoRecieveData.Remove(clientInfo);
        }

        artsSender.StopSession(clientInfo);
    }

    public void EmergencyStopClientSession(string clientInfo)
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

    public void GetDataHistory(string session)
    {
        artsSender.RetrievePreviousSessions(session);
    }
}