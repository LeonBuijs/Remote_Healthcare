using System.Net;
using System.Net.Sockets;

namespace Server;

/**
 * Klasse voor het afhandelen van de verbindingen tussen clients/artsen en de server
 */
public class ConnectionHandler
{
    private static List<Connection> connections = new();

    private IClientCallback clientCallback;
    private IArtsCallback artsCallback;

    public ConnectionHandler(IClientCallback clientCallback, IArtsCallback artsCallback)
    {
        this.clientCallback = clientCallback;
        this.artsCallback = artsCallback;
    }

    public void Start()
    {
        Thread ThreadClient = new Thread(OpenConnectionClient);
        ThreadClient.Start();

        Thread ThreadArts = new Thread(OpenConnectionArts);
        ThreadArts.Start();
    }

    /**
     * Methode voor het verbinden van artsen, er wordt gewacht voor een verbinding en bij een nieuwe verbinding
     * wordt er een nieuwe thread aangemaakt voor die verbinding en start het proces opnieuw
     */
    private void OpenConnectionArts()
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

    /**
     * Methode voor het verbinden van clients, er wordt gewacht voor een verbinding en bij een nieuwe verbinding
     * wordt er een nieuwe thread aangemaakt voor die verbinding en start het proces opnieuw
     */
    private void OpenConnectionClient()
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

    /**
     * Methode voor het afhandelen van requests van de arts, met de callback wordt de server genotificeerd
     */
    private void HandleConnectionArts(Connection connection)
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

    /**
     * Methode voor het afhandelen van requests van de client, met de callback wordt de server genotificeerd
     */
    private void HandleConnectionClient(Connection connection)
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

    /**
     * Methode om te kijken of een client/arts nog verbonden is
     */
    private bool CheckConnection(Connection connectionClient)
    {
        return connectionClient.stream.Socket.Connected;
    }
}