using System.Net;
using System.Net.Sockets;

namespace Server;

/**
 * Klasse voor het afhandelen van de verbindingen tussen clients/doctors en de server
 */
public class ConnectionHandler(IClientCallback clientCallback, IDoctorCallback doctorCallback)
{
    public List<Connection> clients = [];
    public List<Connection> doctors { get; } = [];

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
     * Bij connectie wordt er direct de public key van de connectieClient doorgestuurd
     */
    private void OpenConnectionDoctor()
    {
        var listener = new TcpListener(IPAddress.Any, 7777);
        listener.Start();

        while (true)
        {
            var connectionClient = new Connection(listener.AcceptTcpClient());
            //send public key without encryption
            connectionClient.Send(connectionClient.PublicKeyServerClient, false);
            doctors.Add(connectionClient);
            var threadConnection = new Thread(() => HandleConnectionDoctor(connectionClient));
            threadConnection.Start();
        }
    }

    /**
     * Methode voor het verbinden van clients, er wordt gewacht voor een verbinding en bij een nieuwe verbinding
     * wordt er een nieuwe thread aangemaakt voor die verbinding en start het proces opnieuw
     * Bij connectie wordt er direct de public key van de connectieClient doorgestuurd
     */
    private void OpenConnectionClient()
    {
        var listener = new TcpListener(IPAddress.Any, 6666);
        listener.Start();

        while (true)
        {
            var connectionClient = new Connection(listener.AcceptTcpClient());
            //send public key without encryption
            connectionClient.Send(connectionClient.PublicKeyServerClient, false);
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
        while (true)
        {
            try
            {
                var received = connection.Receive();
                received = received.Trim();
                
                doctorCallback.OnReceivedMessage(received, connection);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
                break;
            }
        }

        doctors.Remove(connection);
    }

    /**
     * Methode voor het afhandelen van requests van de client, met de callback wordt de server genotificeerd
     */
    private void HandleConnectionClient(Connection connection)
    {
        while (true)
        {
            try
            {
                var received = connection.Receive();
                received = received.Trim();

                if (received.Length > 0)
                {
                    clientCallback.OnReceivedMessage(received, connection);

                    Console.WriteLine("Client sent: " + received);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                doctorCallback.OnReceivedMessage("404", connection);
                clientCallback.OnReceivedMessage("404", connection);
                break;
            }
        }

        clients.Remove(connection);
    }
}