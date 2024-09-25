using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace Server;

public class Server : IArtsCallback, IClientCallback
{
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
        //todo requests verwerken en resultaat terugsturen
    }

    void IClientCallback.OnReceivedMessage(string message, Connection connection)
    {
        //todo requests verwerken en resultaat terugsturen
    }
}