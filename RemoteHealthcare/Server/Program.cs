namespace Server;

public class Server : IArtsCallback, IClientCallback
{
    private FileManager fileManager = new();

    // TODO: van groot belang om alle dingen met clients te fixen, worden verkeerd aangeroepen bij doctor
    // TODO: connection veranderen naar String voor index
    Dictionary<string, ClientConnection> clients = new();

    public static void Main(string[] args)
    {
        Console.WriteLine("Starting server...");
        Server server = new Server();
        server.SetCallbacks();
    }

    private void SetCallbacks()
    {
        ConnectionHandler handler = new ConnectionHandler(this, this);
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
    private void DoctorCallbackHandler(Connection connection, string[] messageParts)
    {
        switch (Int32.Parse(messageParts[0]))
        {
            case 0:
                if (CheckLogin(messageParts[1], messageParts[2]))
                {
                    connection.Send("0 1");
                    //stuurt naar verbinden alle clients naar de arts om te tonen
                    SendAllClients(connection);
                }
                else
                {
                    connection.Send("0 0");
                }

                break;
            case 1:
                // Stuur een startcommando naar een specifieke client
                SendCommandToClient(messageParts, "2");
                //client in lijst in sessie zetten
                clients[GetIndexClient(messageParts)].InSession = true;
                // Start van sessie opslaan, is ook naam van het bestand waar alle data van sessie in staat
                clients[GetIndexClient(messageParts)].SessionTime =
                    $"{DateTime.Now.Year}-{DateTime.Now.Day}-{DateTime.Now.Month} " +
                    $"{DateTime.Now.Hour}:{DateTime.Now.Minute}";
                break;
            case 2:
                // Stuur een stopcommando naar een specifieke client
                SendCommandToClient(messageParts, "3");
                clients[GetIndexClient(messageParts)].InSession = false;
                break;
            case 3:
                // Stuur een noodstopcommando naar een specifieke client
                SendCommandToClient(messageParts, "4");
                clients[GetIndexClient(messageParts)].InSession = false;
                break;
            case 4:
                // Stuur een bericht naar een specifieke client
                SendCommandToClient(messageParts, $"0 {messageParts[4]}");
                break;
            case 5:
                // Stuur een bericht naar alle clients
                connection.Send($"5 {messageParts[1]}");
                break;
            case 6:
                // Stuur de weerstand naar een specifieke client
                SendCommandToClient(messageParts, $"1 {messageParts[4]}");
                break;
            case 7:
                // Opgeslagen sessie(s) ophalen van specifieke client
                var sessions = fileManager.getAllClientSessions(GetIndexClient(messageParts));

                //geen sessie beschikbaar van client
                if (sessions == null)
                {
                    connection.Send("3 null");
                }

                foreach (var session in sessions!)
                {
                    connection.Send($"3 {session}");
                }

                break;
            case 8:
                // Alle clients sturen die nu verbonden zijn
                SendAllClients(connection);
                break;
            case 9:
                // Nieuwe client toevoegen aan clientbestand
                fileManager.AddNewClient(GetIndexClient(messageParts));
                break;
            case 10:
                // Live data ontvangen van specifieke client
                //TODO live data tonen van client
                // connection.Send(clients[]);
                break;
        }
    }

    /**
     * Methode om requests van de client af te handelen
     */
    private void ClientCallbackHandler(Connection connection, string[] messageParts)
    {
        switch (Int32.Parse(messageParts[0]))
        {
            case 0:
                // Identificatie bij server
                var index = GetIndexClient(messageParts);
                // Controleert of client bestaat in het clientensbestand, zo ja toevoegen aan lijst met live clients
                if (fileManager.CheckClientLogin(index))
                {
                    clients.Add(GetIndexClient(messageParts), new ClientConnection($"{index}", connection));
                    connection.Send("5 1");
                }
                else
                {
                    connection.Send("5 0");
                }

                break;
            case 1:
                // Fietsdata ontvangen, opslaan en doorsturen naar doctor
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

                var bikeData = ConvertBikeData(messageParts);

                // Live data opslaan in object van client
                clientConnection.LiveData = bikeData;

                // Huidige meting van bikeData opslaan in file
                fileManager.WriteToFile(
                    $"{fileManager.sessionDirectory}/{clientConnection.Name}/{clientConnection.SessionTime}",
                    bikeData);
                break;
        }
    }

    private bool CheckLogin(string username, string password)
    {
        // TODO: Inloggegevens opslaan met encrypt en hier ophalen
        return fileManager.CheckDoctorLogin(username, password);
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

    private string GetIndexClient(string[] messageParts)
    {
        return $"{messageParts[1]} {messageParts[2]} {messageParts[3]}";
    }

    private string ConvertBikeData(string[] messageParts)
    {
        return
            $"{messageParts[1]} {messageParts[2]} {messageParts[3]} {messageParts[4]} {messageParts[5]} {messageParts[6]}";
    }

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