using System.Net;
using System.Net.Sockets;
using System.Text;

namespace VRConnection;

public class ConnectionClient
{
    private TcpListener listener;
    private NetworkStream networkStream;
    private TcpClient client;
    private List<string> dokterMessages = [];

    private void StartServer()
    {
        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
        listener.Start();
        client = listener.AcceptTcpClient();
        networkStream = client.GetStream();
        WaitForCommand();
    }

    private void WaitForCommand()
    {
        while (true)
        {
            var buffer = new byte[1024];
            var bytesRead = networkStream.Read(buffer, 0, buffer.Length);
            var received = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            
            var identifier = received[0];
            var content = received.Substring(2);
            
            switch (identifier)
            {
                case '0':
                    // Message dokter
                    dokterMessages.Add(content);
                    Panel.ChangeChatsPanel(VREngine.uuidPanelChats, dokterMessages);
                    break;
                case '1':
                    // Data
                    string[] data = content.Split(' ');
                    Panel.ChangeSpeedPanel(VREngine.uuidPanelData, Convert.ToInt32(data[0]));
                    Panel.ChangeWattPanel(VREngine.uuidPanelData, Convert.ToInt32(data[1]));
                    Panel.ChangeRPMPanel(VREngine.uuidPanelData, Convert.ToInt32(data[2]));
                    Panel.ChangeHeartRatePanel(VREngine.uuidPanelData, Convert.ToInt32(data[3]));
                    Panel.ChangeTimePanel(VREngine.uuidPanelData, data[4]);
                    Panel.ChangeDistancePanel(VREngine.uuidPanelData, Convert.ToInt32(data[5]));
                    break;
                case '2':
                    // Start command
                    break;
                case '3':
                    // Stop command
                    break;
                case '4':
                    // Emergency stop command
                    break;

            }
        }
    }
}