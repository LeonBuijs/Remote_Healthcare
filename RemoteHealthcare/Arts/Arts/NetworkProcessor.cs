using System.Net.Sockets;
using System.Text;
using SecurityManager;


namespace Arts;

public class NetworkProcessor
{
    private TcpClient artsClient;
    private NetworkStream artsStream;
    private DataSender artsSender;

    private string privateKeyDoctor;
    private string publicKeyDoctor;
    public string? publicKeyServer { set; get; }

    private string ipAdress;

    private readonly ILoginWindowCallback LoginWindowCallback;
    public IListWindowCallback ListWindowCallback { set; get; }

    private List<IDataUpdateCallback> dataUpdateCallbacks = [];
    private List<string> clientsWhoReceiveData = [];
    private bool isAskingData;

    public NetworkProcessor(string ipAddress, ILoginWindowCallback loginWindowWindowCallback)
    {
        artsClient = new TcpClient();
        ipAdress = ipAddress;
        LoginWindowCallback = loginWindowWindowCallback;
        SetRSAKeys();
        ConnectToServer();
    }

    private int setRSAKeyTries = 0;
    /**
     * Genereert en set RSA-sleutels voor de dokter
     */
    private void SetRSAKeys()
    {
        if (setRSAKeyTries >= 5)
        {
            throw new Exception("Failed to set RSA Keys in 5 tries");
        }
        
        var (publicKey, privateKey) = Encryption.GenerateRsaKeyPair();
        publicKeyDoctor = publicKey;
        privateKeyDoctor = privateKey;
        if (string.IsNullOrEmpty(privateKeyDoctor) || string.IsNullOrEmpty(privateKeyDoctor))
        {
            setRSAKeyTries++;
            SetRSAKeys();
        }
    }

    public void ConnectToServer()
    {
        try
        {
            artsClient.Connect(ipAdress, 7777);
            
            //Komt hier niet totdat er een connectie is gemaakt
            Console.WriteLine("Verbonden!");
            artsStream = artsClient.GetStream();
            artsSender = new DataSender(artsStream);

            //Stuurt de public key direct door
            artsSender.SendPublicKey(publicKeyDoctor);
        
            //Verwerk de EncryptionKey die ALTIJD als eerste wordt ontvangen
            byte[] serverSleutel = new byte[1024];
            int bytesRead = artsStream.Read(serverSleutel, 0, serverSleutel.Length);
            publicKeyServer = Encoding.ASCII.GetString(serverSleutel, 0, bytesRead);
            artsSender.publicServerKey = publicKeyServer;
            
            new Thread(ReadSocket).Start();
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
     * Ontvangen data zal gedecodeerd worden.
     */
    private void ReadSocket()
    {
        while (true)
        {
            try
            {
                var buffer = new byte[1024];
                int bytesRead = artsStream.Read(buffer, 0, buffer.Length);
                var result = new byte[bytesRead];
                Array.Copy(buffer, result, bytesRead);
                
                string receivedText = Encryption.DecryptData(result, privateKeyDoctor);
                if (string.IsNullOrEmpty(receivedText))
                {
                    Console.WriteLine("Decrypting failed. Aborting receive");
                    continue;
                }
                Console.WriteLine($"Doctor received decrypted message: \n{receivedText}");
            
                string[] multipleDataReivedSplit = receivedText.Split('\n');
                foreach (var argument in multipleDataReivedSplit)
                {
                    if (string.IsNullOrEmpty(argument))
                    {
                        break;
                    }
                    Console.WriteLine($"argument: {argument}");
                    string[] argumentSplit = argument.Split(" ");

                    if (argumentSplit.Length > 1)
                    {
                        HandleData(argumentSplit);   
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Connection crashed\n{e}");
                break;
            }
        }
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
                string clientId = GetClientIndex(argumentData);
                string data =
                    $"{argumentData[4]} {argumentData[5]} {argumentData[6]} {argumentData[7]} {argumentData[8]} {argumentData[9]}";
                Console.WriteLine($"Got client \"{clientId}\" with data \"{data}\"");

                dataUpdateCallbacks.ForEach(callbackMember => callbackMember.UpdateData(clientId, data));
                break;
            case 2:
                string newClientId = GetClientIndex(argumentData);
                Console.WriteLine($"Kreeg clientId {newClientId}");
                ListWindowCallback.AddNewClient(newClientId);
                break;
            case 3:
                string historyclientId = $"{argumentData[1]} {argumentData[2]} {argumentData[3]}";

                foreach (var clientWindow in dataUpdateCallbacks)
                {
                    if (clientWindow.GetClientinfo().Equals(historyclientId))
                    {
                        string date = argumentData[4];
                        string duration = argumentData[5];
                        string averageSpeed = argumentData[6];
                        string maxSpeed = argumentData[7];
                        string averageHeartRate = argumentData[8];
                        string maxHeartRate = argumentData[9];
                        string distance = argumentData[10];
                        clientWindow.UpdateHistoryTextBlock(date, duration, averageSpeed, maxSpeed, averageHeartRate, maxHeartRate, distance);
                        // {date} {duration}(0) {averageSpeed}(1) {maxSpeed}(1) {averageHeartRate}(2) {maxHeartRate}(2) {distance}(3)
                        clientWindow.UpdateHistoryCharts(0,Int32.Parse(duration), date);
                        clientWindow.UpdateHistoryCharts(1,Int32.Parse(averageSpeed), date);
                        clientWindow.UpdateHistoryCharts(1,Int32.Parse(maxSpeed), date, 1);
                        clientWindow.UpdateHistoryCharts(2,Int32.Parse(averageHeartRate), date);
                        clientWindow.UpdateHistoryCharts(2,Int32.Parse(maxHeartRate), date, 1);
                        clientWindow.UpdateHistoryCharts(3,Int32.Parse(distance), date);
                        Console.WriteLine("1 line of hsitory added");
                    }
                }
                break;
            case 4:
                ListWindowCallback.RemoveClient(GetClientIndex(argumentData));
                RemoveActiveClient(GetClientIndex(argumentData));
                break;
            default:
                Console.WriteLine("Unknown Packet Page");
                break;
        }
    }

    public void AddActiveClient(string clientId)
    {
        lock (clientsWhoReceiveData)
        {
            if (!clientsWhoReceiveData.Contains(clientId))
            {
                clientsWhoReceiveData.Add(clientId);
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
                lock (clientsWhoReceiveData)
                {
                    clientsWhoReceiveData.ForEach(GetRealtimeData);
                    count = clientsWhoReceiveData.Count;
                }

                Thread.Sleep(500);
            }

            isAskingData = false;
        }).Start();
    }

    private void RemoveActiveClient(string clientId)
    {
        lock (clientsWhoReceiveData)
        {
            clientsWhoReceiveData.Remove(clientId);
        }
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
        artsSender.StartSession(clientInfo);
    }

    public void StopClientSession(string clientInfo)
    {
        lock (clientsWhoReceiveData)
        {
            clientsWhoReceiveData.Remove(clientInfo);
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

    private string GetClientIndex(string[] argumentData)
    {
        return $"{argumentData[1]} {argumentData[2]} {argumentData[3]}";
    }
}