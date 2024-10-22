using System.Net;
using System.Net.Sockets;
using System.Text;

namespace VRConnection;

public class ConnectionClient
{
    private static TcpListener listener;
    private static NetworkStream networkStream;
    private static TcpClient client;
    private static List<string> dokterMessages = [];
    private static string name;
    private static double previousSpeed = 0;

    public static void StartServer()
    {
        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
        listener.Start();
        Console.WriteLine("Server Started");
        client = listener.AcceptTcpClient();
        networkStream = client.GetStream();
        Console.WriteLine("Client Connected");
        WaitForCommand();
    }

    private static void WaitForCommand()
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
                    Panel.ClearPanel(VREngine.uuidPanelChats);
                    Panel.ChangeChatsPanel(VREngine.uuidPanelChats, dokterMessages);
                    Panel.ChangeNamePanel(VREngine.uuidPanelChats, name);
                    Panel.SwapPanel(VREngine.uuidPanelChats);
                    break;
                case '1':
                    // Data
                    string[] data = content.Split(' ');
                    if (Convert.ToInt32(data[0]) != previousSpeed)
                    {
                        Route.ChangeFollowRouteSpeed(VREngine.uuidBike, Convert.ToInt32(data[0])/3.6);
                        previousSpeed = Convert.ToInt32(data[0]);
                    }
                    
                    Panel.ClearPanel(VREngine.uuidPanelData);
                    Panel.ChangeSpeedPanel(VREngine.uuidPanelData, Convert.ToInt32(data[0]), 
                        Convert.ToInt32(data[5]), data[3], Convert.ToInt32(data[1]));
                    Panel.SwapPanel(VREngine.uuidPanelData);
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
                case '5':
                    // Naam
                    name = content;
                    Panel.ClearPanel(VREngine.uuidPanelChats);
                    Panel.ChangeNamePanel(VREngine.uuidPanelChats, name);
                    Panel.SwapPanel(VREngine.uuidPanelChats);
                    break;
            }
        }
    }
}