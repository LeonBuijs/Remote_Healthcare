using System.Net;
using System.Net.Sockets;

namespace Server;

public class ConnectionHandler
{
    private List<Connection> connections = new List<Connection>();
    private Server server;
    public ConnectionHandler(Server server)
    {
        this.server = server;
        
        Thread ThreadClient = new Thread(OpenConnectionClient);
        ThreadClient.Start();
        
        Thread ThreadArts = new Thread(OpenConnectionArts);
        ThreadArts.Start();
    }

    private void OpenConnectionClient()
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, 6666);
        listener.Start();
        while (true)
        {
            Connection connectionClient = new Connection(listener.AcceptTcpClient());
            connections.Add(connectionClient);
            Thread ThreadConnection = new Thread(() => HandleConnection(connectionClient));
            ThreadConnection.Start();
        }
    }

    private void OpenConnectionArts()
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, 7777);
        listener.Start();
        while (true)
        {
            Connection connectionClient = new Connection(listener.AcceptTcpClient());
            connections.Add(connectionClient);
            Thread ThreadConnection = new Thread(() => HandleConnection(connectionClient));
            ThreadConnection.Start();
        }
    }

    private void HandleConnection(Connection connection)
    {
        Boolean running = true;
        while (running)
        {
            running = CheckConnection(connection);
        }

        connections.Remove(connection);
    }

    private Boolean CheckConnection(Connection connectionClient)
    {
        return connectionClient.stream.Socket.Connected;
    }
}