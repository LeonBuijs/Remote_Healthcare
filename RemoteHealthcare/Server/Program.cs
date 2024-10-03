namespace Server;

public class Server : IArtsCallback, IClientCallback
{
    private FileManager fileManager = new();
    Dictionary<Connection, ClientConnection> clients = new();

    public static void Main(string[] args)
    {
        Console.WriteLine("Starting server...");
        Server server = new Server();
        server.setCallbacks();
    }

    private void setCallbacks()
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
        ArtsCallbackHandler(connection, messageParts);
    }

    void IClientCallback.OnReceivedMessage(string message, Connection connection)
    {
        var messageParts = message.Split(' ');
        switch (Int32.Parse(messageParts[0]))
        {
            case 0:
                // Sla de gegevens op
                clients.Add(connection, new ClientConnection($"{messageParts[1]} {messageParts[2]} {messageParts[3]}"));
                break;
            case 1:
                if (clients[connection].inSession)
                {
                    //TODO data opslaan van specifieke sessie en doorsturen naar doctor
                }

                // TODO: Sla de fietsdata op en eventueel naar de arts sturen
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

    private void SendCommandToClient(string[] messageParts, string command)
    {
        var requestedClientId = GetIndexClient(messageParts);
        foreach (var client in clients)
        {
            if (requestedClientId.Equals(client.Value.name))
            {
                client.Key.Send(command);
            }
        }
    }

    private void ArtsCallbackHandler(Connection connection, string[] messageParts)
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
                clients[connection].inSession = true;
                clients[connection].sessionTime = $"{DateTime.Now.Year}-{DateTime.Now.Day}-{DateTime.Now.Month} " +
                                                  $"{DateTime.Now.Hour}:{DateTime.Now.Minute}";
                break;
            case 2:
                // Stuur een stopcommando naar een specifieke client
                SendCommandToClient(messageParts, "3");
                clients[connection].inSession = false;
                break;
            case 3:
                // Stuur een noodstopcommando naar een specifieke client
                SendCommandToClient(messageParts, "4");
                clients[connection].inSession = false;
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
                var sessions = fileManager.getAllClientSessions(GetIndexClient(messageParts));

                //geen sessie beschikbaar van client
                if (sessions == null)
                {
                    connection.Send("3 null");
                }

                foreach (var session in sessions)
                {
                    connection.Send($"3 {session}");
                }
                break;
            case 8:
                SendAllClients(connection);
                break;
            case 9:
                fileManager.AddNewClient(GetIndexClient(messageParts));
                break;
            case 10: 
                break;
        }
    }
}