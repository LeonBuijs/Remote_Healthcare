namespace Server;

public class Server : IDoctorCallback, IClientCallback
{
    private readonly FileManager fileManager = new();
    public Dictionary<string, ClientConnection> clients = new();
    private ConnectionHandler connectionHandler;

    public static void Main(string[] args)
    {
        Console.WriteLine("Starting server...");
        var server = new Server();
        server.SetCallbacks();
        Console.WriteLine();
    }

    public void SetCallbacks()
    {
        connectionHandler = new ConnectionHandler(this, this);
        connectionHandler.Start();
    }

    /**
     * Callbacks voor het verwerken van requests vanuit de Doctor en Client(s)
     */
    void IDoctorCallback.OnReceivedMessage(string message, Connection connection)
    {
        var messageParts = message.Split(' ');
        DoctorCallbackHandler(connection, messageParts);
    }

    void IClientCallback.OnReceivedMessage(string message, Connection connection)
    {
        var messageParts = message.Split(' ');
        ClientCallbackHandler(connection, messageParts);
    }

    /**
     * Methode om requests van de doctor af te handelen
     */
    private async void DoctorCallbackHandler(Connection connection, string[] messageParts)
    {
        // Check om te kijken of de doctor toegang heeft, mocht er geen geldige login zijn
        if (messageParts[0] != "0" && !connection.Access)
        {
            return;
        }

        switch ((DoctorDataIndex)int.Parse(messageParts[0].Trim()))
        {
            case DoctorDataIndex.Login:
                DoctorLogin(connection, messageParts);
                break;
            case DoctorDataIndex.StartCommand:
                StartSession(messageParts);
                break;
            case DoctorDataIndex.StopSession:
                await StopSession(messageParts);
                break;
            case DoctorDataIndex.EmergencyStopSession:
                await EmergencyStop(messageParts);
                break;
            case DoctorDataIndex.MessageToSession:
                SendChatMessageToClient(messageParts);
                break;
            case DoctorDataIndex.MessageToAllSessions:
                MessageToAllSessions(messageParts);
                break;
            case DoctorDataIndex.SetBikeSettings:
                SendCommandToClient(messageParts, $"1 {messageParts[4]}");
                break;
            case DoctorDataIndex.RetrieveSessionData:
                await GetSessionData(connection, messageParts);
                break;
            case DoctorDataIndex.RetrieveAllClients:
                SendAllClients(connection);
                break;
            case DoctorDataIndex.AddNewClient:
                fileManager.AddNewClient(GetIndexClient(messageParts));
                break;
            case DoctorDataIndex.RetrieveLiveData:
                SendLiveData(connection, messageParts);
                break;
            case DoctorDataIndex.Disconnected:
                // Mogelijkheid om hier iets te implementeren
                SendClientDisconnected(connection);
                break;
            default:
                Console.WriteLine($"Unknown Doctor Data index: {messageParts[0]}");
                break;
        }
    }

    /**
     * Methode om requests van de client af te handelen
     */
    private void ClientCallbackHandler(Connection connection, string[] messageParts)
    {
        // Check om te kijken of de client toegang heeft, mocht er geen geldige login zijn
        if (messageParts[0] != "0" && !connection.Access)
        {
            Console.WriteLine("Access denied");
            return;
        }

        
        Console.WriteLine("messageparts: " + messageParts[0]);
        
        switch ((ClientDataIndex) int.Parse(messageParts[0]))
        {
            case ClientDataIndex.Login:
                ClientLogin(connection, messageParts);
                break;
            case ClientDataIndex.ReceiveBikeData:
                ReceiveBikeData(connection, messageParts);
                break;
            case ClientDataIndex.Disconnected:
                DisconnectClient(connection);
                break;
        }
    }
    
    /**
     * Helper methode om de login van de Doctor af te handelen
     */
    public void DoctorLogin(Connection connection, string[] messageParts)
    {
        if (fileManager.CheckDoctorLogin(messageParts[1], messageParts[2]))
        {
            connection.Send("0 1");
            connection.Access = true;

            //stuurt naar verbinden alle clients naar de arts om te tonen
            SendAllClients(connection);
        }
        else
        {
            connection.Send("0 0");
        }
    }

    /**
     * Helper methode om een sessie bij een client te starten
     */
    public void StartSession(string[] messageParts)
    {
        var clientIndex = GetIndexClient(messageParts);

        // Check om te controleren of de client bestaat/verbonden is
        if (!clients.ContainsKey(clientIndex))
        {
            return;
        }

        SendCommandToClient(messageParts, "2");
        var clientConnection = clients[clientIndex];

        //client in lijst in sessie zetten
        clientConnection.InSession = true;

        // Start van sessie opslaan, is ook naam van het bestand waar alle data van sessie in staat
        clientConnection.SessionTime =
            $"{DateTime.Now.Year}-{DateTime.Now.Day}-{DateTime.Now.Month}-" +
            $"{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}";
    }

    /**
    * Helper methode om een sessie van een client stop te zetten
    */
    public async Task StopSession(string[] messageParts)
    {
        var clientIndex = GetIndexClient(messageParts);

        // Check om te controleren of de client bestaat/verbonden is
        if (!clients.ContainsKey(clientIndex))
        {
            return;
        }

        var client = clients[clientIndex];

        if (!client.InSession)
        {
            return;
        }

        SendCommandToClient(messageParts, "3");
        client.InSession = false;

        // Asynchroon berekenen van alle fietsdata
        await fileManager.CalculateDataFromSession(client, clientIndex, client.SessionTime);
    }

    /**
     * Helper methode om een noodstop te maken bij een sessie
     */
    public async Task EmergencyStop(string[] messageParts)
    {
        var clientIndex = GetIndexClient(messageParts);

        // Check om te controleren of de client bestaat/verbonden is
        if (!clients.ContainsKey(clientIndex))
        {
            return;
        }

        var client = clients[clientIndex];

        if (!client.InSession)
        {
            return;
        }

        SendCommandToClient(messageParts, "4");
        client.InSession = false;

        // Asynchroon berekenen van alle fietsdata
        await fileManager.CalculateDataFromSession(client, clientIndex, client.SessionTime);
    }

    /**
     * Helper methode om een bericht naar een specifieke client te sturen
     */
    public void SendChatMessageToClient(string[] messageParts)
    {
        var message = "";
        for (int i = 4; i < messageParts.Length; i++)
        {
            message += messageParts[i] + " ";
        }
        
        SendCommandToClient(messageParts, $"0 {message}");
    }

    /**
     * Helper methode om een bericht naar alle sessies te sturen
     */
    public void MessageToAllSessions(string[] messageParts)
    {
        foreach (var client in clients)
        {
            client.Value.Connection.Send($"0 {messageParts[1]}");
        }
    }

    /**
     * Helper methode om data van een bepaalde sessie op te halen
     */
    public async Task GetSessionData(Connection connection, string[] messageParts)
    {
        await Task.Run(() =>
        {
            var clientIndex = GetIndexClient(messageParts);

            var sessions = fileManager.GetAllClientSessions(clientIndex);

            //geen sessie beschikbaar van client
            if (sessions == null)
            {
                connection.Send("3");
                return;
            }

            foreach (var session in sessions)
            {
                Console.WriteLine($"Session: {session}");
                connection.Send($"3 {clientIndex} {session}");
                Thread.Sleep(100);
            }
        });
    }

    /**
     * Helper methode om alle verbonden clients op te halen en te versturen
     */
    public void SendAllClients(Connection connection)
    {
        foreach (var client in clients)
        {
            connection.Send($"2 {client.Key}");
        }
    }

    /**
     * Helper methode om live data van een bepaalde client naar de doctor te sturen
     */
    public void SendLiveData(Connection connection, string[] messageParts)
    {
        var clientIndex = GetIndexClient(messageParts);

        Console.WriteLine($"live data from {clientIndex}!");

        // Check om te controleren of de client bestaat/verbonden is
        if (!clients.ContainsKey(clientIndex))
        {
            Console.WriteLine($"client doesn't exist {clientIndex}");
            return;
        }

        Console.WriteLine($"sent live data {clientIndex} {clients[clientIndex].LiveData}");//todo
        
        connection.Send($"1 {clientIndex} {clients[clientIndex].LiveData}");
    }


    /**
     * Helper methode om het verbreken van een verbinding van een client door te geven
     */
    public void SendClientDisconnected(Connection connection)
    {
        foreach (var doctor in connectionHandler.doctors)
        {
            foreach (var client in clients)
            {
                if (connection.Equals(client.Value.Connection))
                {
                    doctor.Send($"4 {client.Key}");
                }
            }
        }
    }

    /**
     * Helper methode om de login van een client af te handelen
     */
    public void ClientLogin(Connection connection, string[] messageParts)
    {
        var clientIndex = GetIndexClient(messageParts);

        // Controleert of client bestaat in het clientsbestand, zo ja toevoegen aan lijst met live clients
        if (fileManager.CheckClientLogin(clientIndex) && !clients.ContainsKey(clientIndex))
        {
            clients.Add(clientIndex, new ClientConnection(clientIndex, connection));
            connection.Access = true;
            connection.Send("5 1");
        }
        else
        {
            connection.Send("5 0");
        }
    }

    /**
     * Helper methode om ontvangen BikeData te verwerken van een client
     */
    public void ReceiveBikeData(Connection connection, string[] messageParts)
    {
        var clientConnection = GetClientConnection(connection);

        // Mocht er een fout optreden, returnen
        if (clientConnection == null)
        {
            return;
        }

        // Bij ongeldige fietsData, returnen
        // if (messageParts.Length < 7)
        // {
        //     return;
        // }

        var bikeData = $"{messageParts[1]} {messageParts[2]} {messageParts[3]} {messageParts[4]} " +
                       $"{messageParts[5]} {messageParts[6]}";

        // Live data opslaan in object van client
        
        lock (clientConnection) 
        {
            clientConnection.Item1.LiveData = bikeData;
        }

        // Huidige meting van bikeData opslaan in file
        if (clientConnection.Item1.InSession)
        {
            fileManager.WriteToFile(
                $"{fileManager.sessionDirectory}/{clientConnection.Item1.Name}/{clientConnection.Item1.SessionTime}",
                bikeData);
        }
    }

    /**
     * Helper methode om een client te disconnecten van de server
     */
    public void DisconnectClient(Connection connection)
    {
        var client = GetClientConnection(connection);

        //Asynchroon berekenen van alle fietsdata
        Task.Run(async () =>
        {
            await fileManager.CalculateDataFromSession(client.Item1, client.Item2, client.Item1.SessionTime);
        });
        
        
        foreach (var clientName in clients.Keys)
        {
            if (connection.Equals(clients[clientName].Connection))
            {
                clients.Remove(clientName);
            }
        }
    }

    /**
     * Methode om de index (voornaam achternaam geboortedatum) van een client te verkrijgen
     */
    public string GetIndexClient(string[] messageParts)
    {
        if (messageParts.Length < 4)
        {
            return "";
        }

        return $"{messageParts[1]} {messageParts[2]} {messageParts[3]}";
    }

    /**
     * Methode om commando naar client te sturen
     */
    public void SendCommandToClient(string[] messageParts, string command)
    {
        var requestedClientId = GetIndexClient(messageParts);
        foreach (var client in clients)
        {
            if (requestedClientId.Equals(client.Value.Name))
            {
                client.Value.Connection.Send(command);
            }
        }
    }

    public Tuple<ClientConnection, string> GetClientConnection(Connection connection)
    {
        // Door middel van connection kijken welke client het in de lijst is
        foreach (var client in clients)
        {
            if (client.Value.Connection == connection)
            {
                return new Tuple<ClientConnection, string>(client.Value, client.Key);
            }
        }

        return null;
    }
}