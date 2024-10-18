using System.IO.Compression;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using VRConnection;

public class VREngine
{
    private static string SessionID;

    public static void Main(string[] args)
    {
        // Stap 1
        TcpClient tcpClient = new TcpClient();
        tcpClient.Connect("85.145.62.130", 6666);
        tcpClient.ReceiveTimeout = 6000000;
        NetworkStream stream = tcpClient.GetStream();
        Console.WriteLine("Verbonden met de server\n");

        CreateTunnel(stream);
        // Werkende methodes:
        // SetTime(stream, 0);
        DeleteStartingNodes(stream);

        string uuidBike = CreateNodeForBike(stream);
        string uuidTerrain = Terrain.CreateNodeForTerrain(stream);

        string uuidRoute = Route.CreateRoute(stream);
        Route.CreateRoad(stream, uuidRoute);
        Route.FollowRoute(stream, uuidRoute, uuidBike);
        Route.ChangeFollowRouteSpeed(stream, uuidBike, 5.0);
        
        AttachCameraToBike(stream, uuidBike);

        string uuidPanel = Panel.CreateNodeForPanel(stream);
        Panel.ClearPanel(stream, uuidPanel);

        Panel.ChangeNamePanel(stream, uuidPanel, "Name");
        Panel.ChangeSpeedPanel(stream, uuidPanel, 5);
        Panel.ChangeWattPanel(stream, uuidPanel, 100);
        Panel.ChangeRPMPanel(stream, uuidPanel, 25);
        Panel.ChangeHeartRatePanel(stream, uuidPanel, 90);
        Panel.ChangeTimePanel(stream, uuidPanel, "00:00:00");
        Panel.ChangeDistancePanel(stream, uuidPanel, 0);
        Panel.SwapPanel(stream, uuidPanel);
        
        Panel.AttachPanelToBike(stream, uuidPanel, uuidBike);

        Terrain.AddLayerToTerrain(stream, uuidTerrain);

        // Loop voor tijdens de sessie
        while (true)
        {
            RecievePacket(stream);
        }
    }
    
    /**
     * Methode die de camera zoekt en deze vervolgens koppelt aan de uuid van de node die je meegeeft.
     */
    private static void AttachCameraToBike(NetworkStream stream, string uuidBike)
    {
        var cameraUuid = SearchNode(stream, "Camera");

        SendThroughTunnel(stream, "scene/node/update", new
        {
            id = cameraUuid,
            parent = uuidBike,
            transform = new
            {
                position = new[] { -2, 0, 0 },
                scale = 1,
                rotation = new[] { 0, 90, 0 }
            }
        });
        RecievePacket(stream);
    }
    
    /**
     * Methode die een aantal nodes verwijderd die we niet nodig hebben.
     */
    private static void DeleteStartingNodes(NetworkStream stream)
    {
        DeleteNode(stream, SearchNode(stream, "LeftHand"));
        DeleteNode(stream, SearchNode(stream, "RightHand"));
        DeleteNode(stream, SearchNode(stream, "Head"));
        DeleteNode(stream, SearchNode(stream, "GroundPlane"));
    }

    /**
     * Methode om een node te verwijderen, je moet de uuid van de node meegeven die je wilt verwijderen.
     */
    private static void DeleteNode(NetworkStream stream, string uuid)
    {
        SendThroughTunnel(stream, "scene/node/delete", new
        {
            id = uuid
        });
        RecievePacket(stream);
    }
    
    /**
     * Methode om een node te zoeken in de scene, je moet de naam van de node meegeven en je krijgt de uuid terug.
     */
    private static string? SearchNode(NetworkStream stream, string nodeName)
    {
        SendThroughTunnel(stream, "scene/get", null);

        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        JsonArray dataArray = (JsonArray)jsonObject["data"]["data"]["data"]["children"];
        string uuid = null;
        foreach (JsonObject child in dataArray)
        {
            string name = child["name"].ToString();
            if (name == nodeName)
            {
                uuid = child["uuid"].ToString();
            }
        }

        Console.WriteLine($"Node {nodeName} : {uuid}");
        return uuid;
    }
    
    /**
     * Methode die een node aanmaakt voor een fiets, hierdoor wordt er een fiets weergegeven in de simulator.
     */
    private static string CreateNodeForBike(NetworkStream stream)
    {
        SendThroughTunnel(stream, "scene/node/add", new
        {
            name = "Bike",
            components = new
            {
                transform = new
                {
                    position = new[] { 0, 0, 0 },
                    scale = 1,
                    rotation = new[] { 0, 0, 0 },
                },
                model = new
                {
                    file = "data/NetworkEngine/models/bike/bike.fbx"
                }
            }
        });

        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        return jsonObject["data"]["data"]["data"]["uuid"].ToString();
    }
    
    /**
     * Methode die de tijd aanpast in de simulator, je kunt de tijd die je wilt instellen meegeven in uren.
     */
    private static void SetTime(NetworkStream stream, int time)
    {
        SendThroughTunnel(stream, "scene/skybox/settime", new { time = time });
        RecievePacket(stream);
    }

    /**
     * Methode die er voor zorgt dat je een bericht kunt versturen door de tunnel die is aangemaakt.
     * Je moet het commando opgeven die je wilt uitvoeren en de bijbehorende data.
     */
    protected static void SendThroughTunnel(NetworkStream stream, string command, object data)
    {
        var packet = new
        {
            id = "tunnel/send",
            data = new
            {
                dest = SessionID,
                data = new
                {
                    id = command,
                    data = data
                }
            }
        };

        SendPacket(stream, packet);
    }

    /**
     * Methode die een tunnel maakt waarmee je vervolgens data er naar toe kunt sturen.
     */
    private static void CreateTunnel(NetworkStream stream)
    {
        // Stap 2
        SendPacket(stream, new { id = "session/list" });

        // Stap 3
        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        JsonArray dataArray = (JsonArray)jsonObject["data"];

        string ID = null;
        foreach (JsonObject session in dataArray)
        {
            string user = session["clientinfo"]["user"].ToString();
            if (user == System.Environment.UserName)
            {
                ID = session["id"].ToString();
            }
        }

        // Stap 4
        SendPacket(stream, new
        {
            id = "tunnel/create",
            data = new
            {
                session = ID,
            }
        });

        jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        SessionID = jsonObject["data"]["id"].ToString();

        // Stap 5
        SendPacket(stream, new
        {
            id = "tunnel/send",
            data = new
            {
                dest = SessionID,
                data = new
                {
                    id = "scene/reset",
                    data = new { }
                }
            }
        });
        RecievePacket(stream);
    }

    /**
     * Methode die de json die die meekrijgt, stuurt naar de server volgens het protocol.
     */
    private static void SendPacket(NetworkStream stream, object packetString)
    {
        string jsonString = JsonSerializer.Serialize(packetString);
        var array = Encoding.ASCII.GetBytes(jsonString);
        int length = array.Length;
        byte[] prefix = new byte[4];

        // Zet de lengte in de relevante bytes
        prefix[0] = (byte)(length & 0xFF); // Eerste byte (LSB)
        prefix[1] = (byte)((length >> 8) & 0xFF); // Tweede byte
        prefix[2] = (byte)((length >> 16) & 0xFF); // Derde byte
        prefix[3] = (byte)((length >> 24) & 0xFF); // Vierde byte (MSB)      

        byte[] combinedArray = new byte[prefix.Length + array.Length];
        Array.Copy(prefix, 0, combinedArray, 0, prefix.Length);
        Array.Copy(array, 0, combinedArray, prefix.Length, array.Length);
        stream.Write(combinedArray, 0, combinedArray.Length);
        Console.WriteLine(Encoding.ASCII.GetString(combinedArray, 0, combinedArray.Length));
        Console.WriteLine("Bericht verstuurd\n");
    }

    /**
     * Methode die er voor zorgt dat je gegevens kunt ontvangen die je van de server krijgt.
     * Er wordt eerst gekeken hoelang het bericht gaat zijn en haalt vervolgens deze gegevens op.
     */
    protected static string RecievePacket(NetworkStream stream)
    {
        // Uitlezen van de lengte
        var lengthBuffer = new byte[4];
        int bytesToRead = stream.Read(lengthBuffer, 0, lengthBuffer.Length);

        int packetLength = BitConverter.ToInt32(lengthBuffer, 0);
        Console.WriteLine(packetLength);

        int totalBytesRead = 0;
        var buffer = new byte[packetLength];
        var bytesRead = stream.Read(buffer, 0, buffer.Length);
        totalBytesRead += bytesRead;
        while (stream.CanRead && totalBytesRead < packetLength)
        {
            bytesRead = stream.Read(buffer, totalBytesRead, buffer.Length - totalBytesRead);
            totalBytesRead += bytesRead;
        }

        Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, totalBytesRead));
        return Encoding.ASCII.GetString(buffer, 0, totalBytesRead);
    }
}