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
            var indexBuffer = new byte[1];
            var indexBytesRead = networkStream.Read(indexBuffer, 0, indexBuffer.Length);
            var indexReceived = Encoding.ASCII.GetString(indexBuffer, 0, indexBytesRead);
            
            switch (indexReceived)
            {
                case "0":
                    // Message dokter
                    HandleDokterMessage(ReceiveMessage(1024));
                    break;
                case "1":
                    // Data
                    HandleData(ReceiveMessage(6));
                    break;
                case "2":
                    // Start command
                    break;
                case "3":
                    // Stop command
                    break;
                case "4":
                    // Emergency stop command
                    break;
                case "5":
                    // Naam
                    HandleName(ReceiveMessage(1024));
                    break;
            }
        }
    }

    private static string ReceiveMessage(int bufferSize)
    {
        var buffer = new byte[bufferSize];
        var bytesRead = networkStream.Read(buffer, 0, buffer.Length);
        var received = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        return received;
    }

    private static void HandleDokterMessage(string received)
    {
        dokterMessages.Add(received);
        Panel.ClearPanel(VREngine.uuidPanelChats);
        Panel.ChangeChatsPanel(VREngine.uuidPanelChats, dokterMessages);
        Panel.ChangeNamePanel(VREngine.uuidPanelChats, name);
        Panel.SwapPanel(VREngine.uuidPanelChats);
    }

    private static void HandleData(string received)
    {
        string[] data = received.Split(' ');
        if (Convert.ToInt32(data[0]) != previousSpeed)
        {
            Route.ChangeFollowRouteSpeed(VREngine.uuidBike, Convert.ToInt32(data[0])/3.6);
            previousSpeed = Convert.ToInt32(data[0]);
        }
                    
        Panel.ClearPanel(VREngine.uuidPanelData);
        Panel.ChangeDataPanel(VREngine.uuidPanelData, Convert.ToInt32(data[0]), 
            Convert.ToInt32(data[5]), data[3], Convert.ToInt32(data[1]));
        Panel.SwapPanel(VREngine.uuidPanelData);
    }

    private static void HandleName(string received)
    {
        name = received;
        Panel.ClearPanel(VREngine.uuidPanelChats);
        Panel.ChangeNamePanel(VREngine.uuidPanelChats, name);
        Panel.SwapPanel(VREngine.uuidPanelChats);
    }
}