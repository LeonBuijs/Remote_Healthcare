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
        

        public Server()
        {
            Console.WriteLine("Starting server...");
            // ConnectionHandler connectionHandler = new ConnectionHandler();
            ConnectionHandler.Start();
        }

        void IArtsCallback.OnReceivedMessage(string message, Connection connection)
        {
            //todo requests verwerken en resultaat terugsturen
        }

        void IClientCallback.OnReceivedMessage(string message, Connection connection)
        {
            //todo requests verwerken en resultaat terugsturen
        }
    }
}