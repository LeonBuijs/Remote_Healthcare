namespace Server;

public class Server : IArtsCallback, IClientCallback
{
    private FileManager fileManager = new();
    Dictionary<string, ClientConnection> clients = new();
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting server...");
        var server = new Server();
        server.SetCallbacks();
    }

    private void SetCallbacks()
    {
        var handler = new ConnectionHandler(this, this);
        handler.Start();
    }

    /**
     * Callbacks voor het verwerken van requests vanuit de Arts en Client(s)
     */
    void IArtsCallback.OnReceivedMessage(string message, Connection connection)
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

        switch ((DoctorDataIndex)int.Parse(messageParts[0]))
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
                SendCommandToClient(messageParts, $"0 {messageParts[4]}");
                break;
            case DoctorDataIndex.MessageToAllSessions:
                MessageToAllSessions(messageParts);
                break;
            case DoctorDataIndex.SetBikeSettings:
                SendCommandToClient(messageParts, $"1 {messageParts[4]}");
                break;
            case DoctorDataIndex.RetrieveSessionData:
                GetSessionData(connection, messageParts);
                break;
            case DoctorDataIndex.RetrieveAllClients:
                SendAllClients(connection);
                break;
            case DoctorDataIndex.AddNewClient:
                fileManager.AddNewClient(GetIndexClient(messageParts));
                break;
            case DoctorDataIndex.RetrieveLiveData:
                connection.Send(clients[GetIndexClient(messageParts)].LiveData);
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

        switch ((ClientDataIndex)int.Parse(messageParts[0]))
        {
            case ClientDataIndex.Login:
                ClientLogin(connection, messageParts);
                break;
            case ClientDataIndex.ReceiveBikeData:
                ReceiveBikeData(connection, messageParts);
                break;
        }
    }

    /**
     * Helper methode om de login van de Doctor af te handelen
     */
    private void DoctorLogin(Connection connection, string[] messageParts)
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
    private void StartSession(string[] messageParts)
    {
        SendCommandToClient(messageParts, "2");
        var clientConnection = clients[GetIndexClient(messageParts)];

        //client in lijst in sessie zetten
        clientConnection.InSession = true;

        // Start van sessie opslaan, is ook naam van het bestand waar alle data van sessie in staat
        clientConnection.SessionTime =
            $"{DateTime.Now.Year}-{DateTime.Now.Day}-{DateTime.Now.Month} " +
            $"{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}";
    }

    /**
     * Helper methode om een sessie van een client stop te zetten
     */
    private async Task StopSession(string[] messageParts)
    {
        if (!clients[GetIndexClient(messageParts)].InSession)
        {
            return;
        }

        SendCommandToClient(messageParts, "3");
        clients[GetIndexClient(messageParts)].InSession = false;

        // Asynchroon berekenen van alle fietsdata
        await fileManager.CalculateDataFromSession(clients[GetIndexClient(messageParts)],
            GetIndexClient(messageParts),
            clients[GetIndexClient(messageParts)].SessionTime);
    }

    /**
     * Helper methode om een noodstop te maken bij een sessie
     */
    private async Task EmergencyStop(string[] messageParts)
    {
        if (!clients[GetIndexClient(messageParts)].InSession)
        {
            return;
        }

        SendCommandToClient(messageParts, "4");
        clients[GetIndexClient(messageParts)].InSession = false;

        // Asynchroon berekenen van alle fietsdata
        await fileManager.CalculateDataFromSession(clients[GetIndexClient(messageParts)],
            GetIndexClient(messageParts),
            clients[GetIndexClient(messageParts)].SessionTime);
    }

    /**
     * Helper methode om een bericht naar alle sessies te sturen
     */
    private void MessageToAllSessions(string[] messageParts)
    {
        foreach (var client in clients)
        {
            client.Value.Connection.Send($"0 {messageParts[1]}");
        }
    }

    /**
     * Helper methode om data van een bepaalde sessie op te halen
     */
    private void GetSessionData(Connection connection, string[] messageParts)
    {
        var sessions = fileManager.getAllClientSessions(GetIndexClient(messageParts));

        //geen sessie beschikbaar van client
        if (sessions == null)
        {
            connection.Send("3 null");
            return;
        }

        foreach (var session in sessions!)
        {
            connection.Send($"3 {session}");
        }
    }

    /**
     * Helper methode om ontvangen BikeData te verwerken van een client
     */
    private void ReceiveBikeData(Connection connection, string[] messageParts)
    {
        ClientConnection clientConnection = null;

        // Door middel van connection kijken welke client het in de lijst is
        foreach (var client in clients)
        {
            if (client.Value.Connection == connection)
            {
                clientConnection = client.Value;
                break;
            }
        }

        // Mocht er een fout optreden, returnen
        if (clientConnection == null)
        {
            return;
        }

        var bikeData = $"{messageParts[1]} {messageParts[2]} {messageParts[3]} {messageParts[4]} " +
                       $"{messageParts[5]} {messageParts[6]}";

        // Live data opslaan in object van client
        clientConnection.LiveData = bikeData;

        // Huidige meting van bikeData opslaan in file
        if (clientConnection.InSession)
        {
            fileManager.WriteToFile(
                $"{fileManager.sessionDirectory}/{clientConnection.Name}/{clientConnection.SessionTime}",
                bikeData);
        }
    }

    /**
     * Helper methode om de login van een client af te handelen
     */
    private void ClientLogin(Connection connection, string[] messageParts)
    {
        var index = GetIndexClient(messageParts);

        // Controleert of client bestaat in het clientensbestand, zo ja toevoegen aan lijst met live clients
        if (fileManager.CheckClientLogin(index))
        {
            clients.Add(GetIndexClient(messageParts), new ClientConnection($"{index}", connection));
            connection.Access = true;
            connection.Send("5 1");
        }
        else
        {
            connection.Send("5 0");
        }
    }

    /**
     * Methode om alle online clients op te halen en te versturen
     */
    private void SendAllClients(Connection connection)
    {
        foreach (var client in clients)
        {
            connection.Send($"2 {client.Key}");
        }
    }

    /**
     * Methode om de index (voornaam achternaam geboortedatum) van een client te verkrijgen
     */
    private string GetIndexClient(string[] messageParts)
    {
        return $"{messageParts[1]} {messageParts[2]} {messageParts[3]}";
    }

    /**
     * Methode om commando naar client te sturen
     */
    private void SendCommandToClient(string[] messageParts, string command)
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
}