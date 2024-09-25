using System.Net;
using System.Net.Sockets;

namespace Server;

public static class ConnectionHandler
{
    private static List<Connection> connections = new();
    
    private static IClientCallback clientCallback;
    private static IArtsCallback artsCallback;

    public static void Start()
    {
        Thread ThreadClient = new Thread(OpenConnectionClient);
        ThreadClient.Start();
        
        Thread ThreadArts = new Thread(OpenConnectionArts);
        ThreadArts.Start();
    }

    private static void OpenConnectionClient()
    {
        Console.WriteLine("Client Connection thread opened");
        TcpListener listener = new TcpListener(IPAddress.Loopback, 6666);
        listener.Start();
        while (true)
        {
            Connection connectionClient = new Connection(listener.AcceptTcpClient());
            connections.Add(connectionClient);
            Thread ThreadConnection = new Thread(() => HandleConnectionClient(connectionClient));
            ThreadConnection.Start();
        }
    }

    private static void OpenConnectionArts()
    {
        Console.WriteLine("Arts Connection thread opened");
        TcpListener listener = new TcpListener(IPAddress.Loopback, 7777);
        listener.Start();
        while (true)
        {
            Connection connectionClient = new Connection(listener.AcceptTcpClient());
            connections.Add(connectionClient);
            Thread ThreadConnection = new Thread(() => HandleConnectionArts(connectionClient));
            ThreadConnection.Start();
        }
    }

    private static void HandleConnectionArts(Connection connection)
    {
        Boolean running = true;
        while (running)
        {
            running = CheckConnection(connection);
            var received = connection.Receive();
            artsCallback.OnReceivedMessage(received, connection);
            
            Console.WriteLine("Arts sent: " + received);
        }

        connections.Remove(connection);
    }
    private static void HandleConnectionClient(Connection connection)
    {
        Boolean running = true;
        while (running)
        {
            running = CheckConnection(connection);
            
            var received = connection.Receive();
            clientCallback.OnReceivedMessage(connection.Receive(), connection);
            
            Console.WriteLine("Client sent: " + received);
        }

        connections.Remove(connection);
    }

    private static bool CheckConnection(Connection connectionClient)
    {
        return connectionClient.stream.Socket.Connected;
    }
}