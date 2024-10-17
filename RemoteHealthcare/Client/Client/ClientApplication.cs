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
        private BikeDataSender _bikeDataSender;
        private BikeData _bikeData;
        
        public ClientApplication(string ipAddress, int port, BikeData bikeData)
        {
            // Verbind met server
            _client = new TcpClient(ipAddress, port);
            _stream = _client.GetStream();

            _bikeData = bikeData;
            _bikeDataSender = new BikeDataSender(_client, _bikeData);
        }
        
        public async Task Start()
        {
            Console.WriteLine("Verbonden met de server.");

            // Begin met het verzenden van fietsdata naar server
            await _bikeDataSender.SendBikeData();
        }

        public async Task SendMessage(string message)
        {
            var data = Encoding.ASCII.GetBytes(message);
            await _stream.WriteAsync(data, 0, data.Length);
        }
        
        public void CloseConnection()
        {
            _stream.Close();
            _client.Close();
            Console.WriteLine("Verbinding gesloten.");
        }
    }
}