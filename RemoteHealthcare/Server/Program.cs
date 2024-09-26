using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace Server;

public class Server : IArtsCallback, IClientCallback
{
    private List<String> clients = new List<String>();
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting server...");
        ConnectionHandler.Start();
    }

    /**
     * Callbacks voor het verwerken van requests vanuit de Arts en Client(s)
     */
    void IArtsCallback.OnReceivedMessage(string message, Connection connection)
    {
        var messageParts = message.Split(' ');
        switch (Int32.Parse(messageParts[0]))
        {
            case 0:
                if (CheckLogin(messageParts[1], messageParts[2]))
                {
                    connection.Send("0 1");
                    foreach (var client in clients)
                    {
                        connection.Send(client);
                    }
                }
                else
                {
                    connection.Send("0 0");
                }
                break;
            case 1:
                // TODO: Stuur een startcommando naar de specifieke client
                
                break;
            case 2:
                // TODO: Stuur een stopcommando naar de specifieke client
                break;
            case 3:
                // TODO: Stuur een noodstopcommando naar de specifieke client
                break;
            case 4:
                // TODO: Stuur een bericht naar een specifieke client
                break;
            case 5:
                // TODO: Stuur een bericht naar alle clients
                break;
            case 6:
                // TODO: Stuur de weerstand naar een specifieke client
                break;
            case 7:
                // TODO: Stuur de gevraagde data terug naar de arts
                break;
            
        }
    }

    void IClientCallback.OnReceivedMessage(string message, Connection connection)
    {
        var messageParts = message.Split(' ');
        String client = "";
        switch (Int32.Parse(messageParts[0]))
        {
            case 0:
                // TODO: Sla de gegevens op
                client += messageParts[1];
                break;
            case 1:
                // TODO: Sla de fietsdata op en eventueel naar de arts sturen
                break;
        }
    }

    private Boolean CheckLogin(string Username, string Password)
    {
        // TODO: Inloggegevens opslaan met encrypt en hier ophalen
        return (Username == "admin" && Password == "admin");
    }
}