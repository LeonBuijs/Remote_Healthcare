using System.Net.Sockets;
using System.Text;
using Avans.TI.BLE;

namespace ClientGUI
{
    public class ClientApplication
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private BikeDataSender _bikeDataSender;
        private BikeData _bikeData;
        private MessageHandler _messageHandler;

        public NetworkStream Stream => _stream; // For MessageHandler class
        

        public ClientApplication(string ipAddress, int port, BLE bleBike = null)
        {
            _client = new TcpClient(ipAddress, port);
            _stream = _client.GetStream();
            _bikeData = new BikeData();
            _bikeDataSender = new BikeDataSender(_client, _bikeData);
            _messageHandler = new MessageHandler(this, _stream, bleBike);
        }

        public async Task Start()
        {
            await _bikeDataSender.SendBikeData();
            await _messageHandler.ReceiveMessages();
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
            Console.WriteLine("Connection closed.");
        }
    }
}