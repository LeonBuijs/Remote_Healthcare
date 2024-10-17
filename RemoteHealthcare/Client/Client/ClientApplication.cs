using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientApplication
    {
        private TcpClient _client;
        public NetworkStream _stream;
        private BikeData _bikeData;
        
        public ClientApplication(string ipAddress, int port, BikeData bikeData)
        {
            // Verbind met server
            _client = new TcpClient(ipAddress, port);
            _stream = _client.GetStream();

            _bikeData = bikeData;
        }
        

        public void SendMessage(string message)
        {
            var data = Encoding.ASCII.GetBytes(message);
            _stream.Write(data, 0, data.Length);
        }
        
        public void CloseConnection()
        {
            _stream.Close();
            _client.Close();
            Console.WriteLine("Verbinding gesloten.");
        }
    }
}