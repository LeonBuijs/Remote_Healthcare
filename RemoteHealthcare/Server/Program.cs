namespace Server;

public class Server : IArtsCallback, IClientCallback
{
    Dictionary<String, Connection> clients = new Dictionary<String, Connection>();
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
                clients.Add($"{messageParts[1]} {messageParts[2]} {messageParts[3]}", connection);
                break;
            case 1:
                // TODO: Sla de fietsdata op en eventueel naar de arts sturen
                break;
        }
    }

    private Boolean CheckLogin(string username, string password)
    {
        // TODO: Inloggegevens opslaan met encrypt en hier ophalen
        return (username == "admin" && password == "admin");
    }

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
            if (requestedClientId.Equals(client.Key))
            {
                client.Value.Send(command);
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
                break;
            case 2:
                // Stuur een stopcommando naar een specifieke client
                SendCommandToClient(messageParts, "3");
                break;
            case 3:
                // Stuur een noodstopcommando naar een specifieke client
                SendCommandToClient(messageParts, "4");
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
                // TODO: Stuur de gevraagde data terug naar de arts
                break;
            case 8:
                SendAllClients(connection);
                break;
        }
    }
}