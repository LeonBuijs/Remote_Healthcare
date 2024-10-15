using System.Net;
using System.Net.Sockets;

namespace Server;

/**
 * Klasse voor het afhandelen van de verbindingen tussen clients/doctors en de server
 */
public class ConnectionHandler(IClientCallback clientCallback, IDoctorCallback doctorCallback)
{
    private static readonly List<Connection> clients = [];
    private static readonly List<Connection> doctors = [];

    public void Start()
    {
        var threadClient = new Thread(OpenConnectionClient);
        threadClient.Start();

        var threadDoctor = new Thread(OpenConnectionDoctor);
        threadDoctor.Start();
    }

    /**
     * Methode voor het verbinden van doctoren, er wordt gewacht voor een verbinding en bij een nieuwe verbinding
     * wordt er een nieuwe thread aangemaakt voor die verbinding en start het proces opnieuw
     */
    private void OpenConnectionDoctor()
    {
        var listener = new TcpListener(IPAddress.Loopback, 7777);
        listener.Start();
        
        while (true)
        {
            var connectionClient = new Connection(listener.AcceptTcpClient());
            doctors.Add(connectionClient);
            var threadConnection = new Thread(() => HandleConnectionDoctor(connectionClient));
            threadConnection.Start();
        }
    }

    /**
     * Methode voor het verbinden van clients, er wordt gewacht voor een verbinding en bij een nieuwe verbinding
     * wordt er een nieuwe thread aangemaakt voor die verbinding en start het proces opnieuw
     */
    private void OpenConnectionClient()
    {
        var listener = new TcpListener(IPAddress.Loopback, 6666);
        listener.Start();
        
        while (true)
        {
            var connectionClient = new Connection(listener.AcceptTcpClient());
            clients.Add(connectionClient);
            var threadConnection = new Thread(() => HandleConnectionClient(connectionClient));
            threadConnection.Start();
        }
    }

    /**
     * Methode voor het afhandelen van requests van de doctor, met de callback wordt de server genotificeerd
     */
    private void HandleConnectionDoctor(Connection connection)
    {
        Boolean running = true;
        while (running)
        {
            running = CheckConnection(connection);

            var received = connection.Receive();
            doctorCallback.OnReceivedMessage(received, connection);

            Console.WriteLine("Doctor sent: " + received);
        }

        doctors.Remove(connection);
    }

    /**
     * Methode voor het afhandelen van requests van de client, met de callback wordt de server genotificeerd
     */
    private void HandleConnectionClient(Connection connection)
    {
        var running = true;
        while (running)
        {
            running = CheckConnection(connection);
            
            var received = connection.Receive();
            clientCallback.OnReceivedMessage(received, connection);

            Console.WriteLine("Client sent: " + received);
        }

        clients.Remove(connection);
    }

    /**
     * Methode om te kijken of een client/doctor nog verbonden is
     */
    private static bool CheckConnection(Connection connectionClient)
    {
        return connectionClient.Stream.Socket.Connected;
    }
}