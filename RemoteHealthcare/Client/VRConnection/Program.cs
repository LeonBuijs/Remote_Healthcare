using System.IO.Compression;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;

class VRConnection
{
    private static string SessionID;

    public static void Main(string[] args)
    {
        // Stap 1
        TcpClient tcpClient = new TcpClient();
        tcpClient.Connect("85.145.62.130", 6666);
        tcpClient.ReceiveTimeout = 10000;
        NetworkStream stream = tcpClient.GetStream();
        Console.WriteLine("Verbonden met de server\n");

        CreateTunnel(stream);
        // Werkende methodes:
        // SetTime(stream, 0);

        string uuidBike = CreateNodeForBike(stream);
        // string uuidTerrain = CreateNodeForTerrain(stream);
        
        string uuidRoute = CreateRoute(stream);
        CreateRoad(stream, uuidRoute);
        
        FollowRoute(stream, uuidRoute, uuidBike);
        //
        ChangeFollowRouteSpeed(stream, uuidBike, 50.0);
        //
        AttachCameraToBike(stream, uuidBike);


        // SendPacket(stream, "{\"id\" : " +
        //                    "\"tunnel/send\", " +
        //                    "\"data\" :" +
        //                    "{\"dest\" : \"" +
        //                    SessionID +
        //                    "\", " +
        //                    "\"data\" :" +
        //                    "{\"id\" : \"scene/panel/drawtext\", " +
        //                    "\"data\" : " +
        //                    "{\"id\" : " + uuid + ", " +
        //                    "\"text\" : \"Hello World\", " +
        //                    "\"position\" : [ 0.0, 0.0 ]" +
        //                    "}}}}");
        // RecievePacket(stream);
    }

    private static void AttachCameraToBike(NetworkStream stream, string uuidBike)
    {
        SendThroughTunnel(stream, "scene/get", null);
        
        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        JsonArray dataArray = (JsonArray)jsonObject["data"]["data"]["data"]["children"];
        string cameraUuid = null;
        foreach (JsonObject child in dataArray)
        {
            string name = child["name"].ToString();
            if (name == "Camera")
            {
                cameraUuid = child["uuid"].ToString();
            }
        }
        // TODO: Positie verbeteren
        SendThroughTunnel(stream, "scene/node/update", new
        {
            id = cameraUuid,
            parent = uuidBike,
            transform = new
            {
                position = new[] {1,0,2},
                scale = 1,
                rotation = new[] {0, 90, 0}
            }
        });
        RecievePacket(stream);
    }
    // private static void GenerateTerrain(int width, int height, int[] terrainMap)
    // {
    //     float scale = 20f;  // Bepaalt hoe "heuvelachtig" het terrein is
    //     int maxHeight = 20; // Maximale hoogte van het terrein
    //
    //     for (int y = 0; y < height; y++)
    //     {
    //         for (int x = 0; x < width; x++)
    //         {
    //             float sampleX = x / scale;
    //             float sampleY = y / scale;
    //             float noiseValue = PerlinNoise(sampleX, sampleY);
    //             
    //             // Schaal de noise waarde naar een geheel getal voor het terrein
    //             terrainMap[y * width + x] = (int)((noiseValue + 1) * (maxHeight / 2)); // Zorgt ervoor dat de minimale waarde 0 is
    //         }
    //     }
    // }
    //
    // // Simpele noise functie
    // private static float PerlinNoise(float x, float y)
    // {
    //     return (float)(Math.Sin(x) + Math.Sin(y)) / 2;  // De waarden liggen tussen -1 en 1
    // }
    //
    // private static string ConvertArrayToJson(int[] array)
    // {
    //     StringBuilder jsonBuilder = new StringBuilder();
    //     jsonBuilder.Append("[");
    //     
    //     for (int i = 0; i < array.Length; i++)
    //     {
    //         jsonBuilder.Append(array[i]);
    //         if (i < array.Length - 1)
    //         {
    //             jsonBuilder.Append(",");
    //         }
    //     }
    //     
    //     jsonBuilder.Append("]");
    //     return jsonBuilder.ToString();
    // }

    private static void ChangeFollowRouteSpeed(NetworkStream stream, string uuidBike, double speed)
    {
        SendThroughTunnel(stream, "route/follow/speed", new { node = uuidBike, speed = speed });
        RecievePacket(stream);
    }

    private static void FollowRoute(NetworkStream stream, string uuidRoute, string uuidBike)
    {
        SendThroughTunnel(stream, "route/follow", new
        {
            route = uuidRoute,
            node = uuidBike,
            speed = 10.0,
            offset = 0.0,
            rotate = "XZ",
            smoothing = 1.0,
            followHeight = false,
            rotateOffset = new[] { 0, 0, 0 },
            positionOffset = new[] { 0, 0, 0 }
        });
        
        RecievePacket(stream);
    }

    private static void CreateRoad(NetworkStream stream, string uuidRoute)
    {
        SendThroughTunnel(stream, "scene/road/add", new { route = uuidRoute });
        RecievePacket(stream);
    }

    private static string CreateRoute(NetworkStream stream)
    {
        SendThroughTunnel(stream, "route/add", new
        {
            nodes = new[]
            {
                new { pos = new[] { 0, 0, 0 }, dir = new[] { 5, 0, -5 }, },
                new { pos = new[] { 50, 0, 0 }, dir = new[] { 5, 0, 5 }, },
                new { pos = new[] { 50, 0, 50 }, dir = new[] { -5, 0, 5 }, },
                new { pos = new[] { 0, 0, 50 }, dir = new[] { -5, 0, -5 }, }
            }
        });

        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        return jsonObject["data"]["data"]["data"]["uuid"].ToString();
    }

    private static void createTerrain(NetworkStream stream)
    {
        // Heuvellandschap

        // int[] terrainMap = new int[256 * 256];
        // GenerateTerrain(256, 256, terrainMap);
        // string jsonString = ConvertArrayToJson(terrainMap);


        int[] heights = new int[256 * 256];

        SendThroughTunnel(stream, "scene/terrain/add", new
        {
            size = new int[] { 256, 256 },
            heights = heights
        });

        RecievePacket(stream);
    }

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

    private static string CreateNodeForTerrain(NetworkStream stream)
    {
        createTerrain(stream);
        SendThroughTunnel(stream, "scene/node/add", new
        {
            name = "Terrain",
            components = new
            {
                transform = new
                {
                    position = new[] { 0, 0, 0 },
                    scale = 1,
                    rotation = new[] { 0, 0, 0 }
                },
                terrain = new
                {
                    smoothnormals = true
                }
            }
        });

        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        return jsonObject["data"]["data"]["data"]["uuid"].ToString();
    }

    private static void SetTime(NetworkStream stream, int time)
    {
        SendThroughTunnel(stream, "scene/skybox/settime", new { time = time });
        RecievePacket(stream);
    }

    private static void SendThroughTunnel(NetworkStream stream, string command, object data)
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

    private static string RecievePacket(NetworkStream stream)
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