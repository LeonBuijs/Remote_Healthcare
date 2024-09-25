using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class Server : IArtsCallback, IClientCallback
    {
        public static void Main(string[] args)
        {
            Server s = new Server();
        }
        
        ConnectionHandler connectionHandler;

        public Server()
        {
            Console.WriteLine("Starting server...");
            connectionHandler = new ConnectionHandler(this);
        }

        void IArtsCallback.OnReceivedMessage(string message, Connection connection)
        {
            throw new NotImplementedException();
        }

        void IClientCallback.OnReceivedMessage(string message, Connection connection)
        {
            throw new NotImplementedException();
        }
    }
}