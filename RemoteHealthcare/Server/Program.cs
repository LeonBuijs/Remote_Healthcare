using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Server
    {
        private static List<Connection> connections = new List<Connection>();
        public static void Main(string[] args)
        {
            Thread ThreadClient = new Thread(OpenConnectionClient);
            ThreadClient.Start();
            Thread ThreadArts = new Thread(OpenConnectionArts);

        }
        
        private static void OpenConnectionClient()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 6666);
            while (true)
            {
                Connection connectionClient = new Connection(listener.AcceptTcpClient());
                connections.Add(connectionClient);
                Thread ThreadConnection = new Thread(() => HandleConnection(connectionClient));
                ThreadConnection.Start();
            }
        }
        private static void OpenConnectionArts()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 7777);
            while (true)
            {
                Connection connectionClient = new Connection(listener.AcceptTcpClient());
                connections.Add(connectionClient);
                Thread ThreadConnection = new Thread(() => HandleConnection(connectionClient));
                ThreadConnection.Start();
            }
        }

        private static void HandleConnection(Connection connection)
        {
            Boolean running = true;
            while (running)
            {
                running = CheckConnection(connection);
            }
            connections.Remove(connection);
        }

        private static Boolean CheckConnection(Connection connectionClient)
        {
            return connectionClient.stream.Socket.Connected;

        }
    }
}

