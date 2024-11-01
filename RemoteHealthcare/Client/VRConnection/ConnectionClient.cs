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

    private static Queue<string> CommandQueue = new();

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
            var buffer = new byte[512];
            string received;

            try
            {
                var bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                received = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            }
            catch (Exception)
            {
                HandleMessage("  --- CLIENT DISCONNECTED ---");
                client = listener.AcceptTcpClient();
                networkStream = client.GetStream();
                dokterMessages.Clear();
                continue;
            }


            Console.WriteLine($"----------\nReceived: {received}\n----------");

            var commands = received.Split('\n');

            // variabele om te controleren of de buffer een bericht van de doctor bevat,
            // zo ja op true zetten en de index meegeven
            var containsDoctorCommand = false;
            for (int i = 0; i < commands.Length; i++)
            {
                var command = commands[i];

                if (!command.StartsWith("1") && command != "")
                {
                    Console.WriteLine($"True Command: {command}");
                    CommandQueue.Enqueue(command);
                    containsDoctorCommand = true;
                }
            }

            Console.WriteLine($"Doctor Command: {containsDoctorCommand}");

            if (!containsDoctorCommand)
            {
                var commandPosition = commands.Length - 2;

                if (commandPosition < 0)
                {
                    commandPosition = 0;
                }

                if (commands.Length > commandPosition && CommandQueue.Count <= 1)
                {
                    CommandQueue.Enqueue(commands[commandPosition]);
                }
            }

            var toHandle = CommandQueue.Dequeue();

            if (toHandle != "")
            {
                HandleCommand(toHandle);
            }
        }
    }

    private static void HandleMessage(string received)
    {
        dokterMessages.Add(received.Substring(2));
        Panel.ClearPanel(VREngine.uuidPanelChats);
        Panel.ChangeChatsPanel(VREngine.uuidPanelChats, dokterMessages, [0, 0, 0, 1]);
        Panel.ChangeNamePanel(VREngine.uuidPanelChats, name);
        Panel.SwapPanel(VREngine.uuidPanelChats);
    }

    private static void HandleCommand(string received)
    {
        Console.WriteLine($"identifier: {received}");
        var identifier = received[0];

        switch (identifier)
        {
            case '0':
                // Message dokter
                HandleMessage(received);
                break;
            case '1':
                // Data
                string[] data = received.Substring(2).Split(' ');
                if (Convert.ToInt32(data[0]) != previousSpeed)
                {
                    Route.ChangeFollowRouteSpeed(VREngine.uuidBike, Convert.ToInt32(data[0]) / 3.6);
                    previousSpeed = Convert.ToInt32(data[0]);
                }

                Panel.ClearPanel(VREngine.uuidPanelData);
                Panel.ChangeDataPanel(VREngine.uuidPanelData, Convert.ToInt32(data[0]),
                    Convert.ToInt32(data[5]), data[3], Convert.ToInt32(data[1]));
                Panel.SwapPanel(VREngine.uuidPanelData);
                break;
            case '2':
                // Start command
                HandleStartStop(received);
                break;
            case '3':
                // Stop command
                HandleStartStop(received);
                Route.ChangeFollowRouteSpeed(VREngine.uuidBike, 0);
                break;
            case '4':
                // Emergency stop command
                HandleStartStop(received);
                Route.ChangeFollowRouteSpeed(VREngine.uuidBike, 0);
                break;
            case '5':
                // Naam
                name = received.Substring(2);
                Panel.ClearPanel(VREngine.uuidPanelChats);
                Panel.ChangeNamePanel(VREngine.uuidPanelChats, name);
                Panel.SwapPanel(VREngine.uuidPanelChats);
                break;
        }
    }

    private static void HandleStartStop(string received)
    {
        switch (received)
        {
            case "2":
                HandleMessage("  --- START SESSION ---");
                break;
            case "3":
                HandleMessage("  --- STOP SESSION ---");
                break;
            case "4":
                // HandleMessage("  --- EMERGENCY STOP! ---");
                dokterMessages.Add("  --- EMERGENCY STOP! ---");
                Panel.ClearPanel(VREngine.uuidPanelChats);
                Panel.ChangeChatsPanel(VREngine.uuidPanelChats, dokterMessages, [255, 0, 0, 1]);
                Panel.ChangeNamePanel(VREngine.uuidPanelChats, name);
                Panel.SwapPanel(VREngine.uuidPanelChats);
                break;
        }
    }
}