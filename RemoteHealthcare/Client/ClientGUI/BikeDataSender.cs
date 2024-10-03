using System.Net.Sockets;
using System.Text;

namespace ClientGUI
{
    public class BikeDataSender
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private BikeData _bikeData;
        
        public BikeDataSender(TcpClient client, BikeData bikeData)
        {
            _client = client;
            _stream = client.GetStream();
            _bikeData = bikeData;
        }
        
        // Stuur de fietsdata naar server
        public async Task SendBikeData()
        {
            while (true)
            {
                string bikeDataMessage = FormatBikeData();
                SendMessage(bikeDataMessage);
                
                await Task.Delay(5000);
            }
        }
        
        private string FormatBikeData()
        {
            return $"Snelheid: {_bikeData.speed}, Afstand: {_bikeData.distance}, Vermogen: {_bikeData.watt}, Tijd: {_bikeData.time}, RPM: {_bikeData.rpm}, Hartslag: {_bikeData.heartRate}";
        }

        // Bericht naar server sturen
        private void SendMessage(string message)
        {
            var data = Encoding.ASCII.GetBytes(message);
            _stream.Write(data, 0, data.Length);
        }
    }
}