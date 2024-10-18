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
        
        ChangeFollowRouteSpeed(stream, uuidBike, 50.0);
        
        AttachCameraToBike(stream, uuidBike);

        // string uuidPanel = CreateNodeForPanel(stream);
        // DrawTextOnPanel(stream, "Hello World", uuidPanel);
        // SwapPanel(stream, uuidPanel);
    }

    /**
     * Methode om meegegeven tekst weer te geven op het meegegeven panel.
     */
    private static void DrawTextOnPanel(NetworkStream stream, string text, string uuidPanel)
    {
        ClearPanel(stream, uuidPanel);
        SendThroughTunnel(stream, "scene/panel/drawtext", new
        {
            id = uuidPanel,
            text = text,
            position = new[] { 10, 100 },
            size = 32,
            color = new[] { 0, 0, 0, 1 }
        });
        RecievePacket(stream);
    }

    /**
     * Methode om het meegegeven panel te clearen.
     */
    private static void ClearPanel(NetworkStream stream, string uuid)
    {
        SendThroughTunnel(stream, "scene/panel/clear", new { id = uuid });
        RecievePacket(stream);
    }

    /**
     * Methode die de buffer wisselt van het panel die je meegeeft,
     * dit zorgt er eigenlijk voor dat het panel geupdate wordt.
     */
    private static void SwapPanel(NetworkStream stream, string uuid)
    {
        SendThroughTunnel(stream, "scene/panel/swap", new
        {
            id = uuid
        });
        RecievePacket(stream);
    }

    /**
     * Methode die een node maakt waardoor het panel weergegeven kan worden.
     */
    private static string CreateNodeForPanel(NetworkStream stream)
    {
        SendThroughTunnel(stream, "scene/node/add", new
        {
            name = "Panel",
            components = new
            {
                transform = new
                {
                    position = new[] { 1, 2, 0 },
                    scale = 1,
                    rotation = new[] { 0, 0, 0 },
                },
                panel = new
                {
                    size = new[] { 1, 2 },
                    resolution = new[] { 256, 512 },
                    background = new[] { 1, 1, 1, 1 },
                    castShadow = true
                }
            }
        });

        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        return jsonObject["data"]["data"]["data"]["uuid"].ToString();
    }

    /**
     * 1. Deze methode zoekt de camera node.
     * <p> 2. De camera node wordt gekoppeld aan de node (uuid) die je meegeeft, bijvoorbeeld de fiets. </p>
     */
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
                position = new[] { -2, 0, 0 },
                scale = 1,
                rotation = new[] { 0, 90, 0 }
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

    /**
     * Deze methode past de snelheid aan waarmee een node een route volgt,
     * je geeft de uuid van de node mee met de snelheid die je wilt instellen.
     */
    private static void ChangeFollowRouteSpeed(NetworkStream stream, string uuidBike, double speed)
    {
        SendThroughTunnel(stream, "route/follow/speed", new { node = uuidBike, speed = speed });
        RecievePacket(stream);
    }

    /**
     * Deze methode laat een node een route volgen,
     * je geeft de uuid van de route mee en de uuid van de node die deze route moet volgen.
     */
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

    /**
     * Deze methode maakt een road aan op een route, je geeft de uuid van de route mee.
     */
    private static void CreateRoad(NetworkStream stream, string uuidRoute)
    {
        SendThroughTunnel(stream, "scene/road/add", new { route = uuidRoute });
        RecievePacket(stream);
    }

    /**
     * Deze methode maakt een route aan door middel van een aantal punten.
     */
    private static string CreateRoute(NetworkStream stream)
    {
        SendThroughTunnel(stream, "route/add", new
        {
            nodes = new[]
            {
                new { pos = new[] { 0, 0, 0 }, dir = new[] { 5, 0, 0 }},
                new { pos = new[] { 40, 0, 0 }, dir = new[] { 5, 0, 2 }},
                new { pos = new[] { 75, 0, 20 }, dir = new[] { 3, 0, 3 }},
                new { pos = new[] { 100, 0, 50 }, dir = new[] { 0, 0, 5 }},
                new { pos = new[] { 100, 0, 100 }, dir = new[] { -3, 0, 3 }},
                new { pos = new[] { 75, 0, 125 }, dir = new[] { -5, 0, 2 }},
                new { pos = new[] { 40, 0, 140 }, dir = new[] { -5, 0, -2 }},
                new { pos = new[] { 0, 0, 125 }, dir = new[] { -3, 0, -3 }},
                new { pos = new[] { -25, 0, 100 }, dir = new[] { 0, 0, -5 }},
                new { pos = new[] { -25, 0, 50 }, dir = new[] { 3, 0, -3 }},
                new { pos = new[] { 0, 0, 25 }, dir = new[] { 5, 0, 0 }}
            }
        });

        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        return jsonObject["data"]["data"]["data"]["uuid"].ToString();
    }

    /**
     * Deze methode maakt een terrein aan van 256 x 256 met een bepaalde hoogte.
     */
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

    /**
     * Deze methode maakt een node aan voor een fiets, hierdoor wordt er een fiets weergegeven in de simulator.
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
     * Deze methode maakt een node aan voor een terrein, hierdoor kan het terrein weergegeven worden in de simulator.
     */
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

    /**
     * Deze methode past de tijd aan in de simulator, je kunt de tijd die je wilt instellen meegeven in uren.
     */
    private static void SetTime(NetworkStream stream, int time)
    {
        SendThroughTunnel(stream, "scene/skybox/settime", new { time = time });
        RecievePacket(stream);
    }

    /**
     * Deze methode zorgt ervoor dat je een bericht kunt versturen door de tunnel die is aangemaakt.
     * Je moet het commando opgeven die je wilt uitvoeren en de bijbehorende data.
     */
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

    /**
     * Deze methode maakt een tunnel waarmee je vervolgens data er naar toe kunt sturen.
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
     * Deze methode stuurt de json die die meekrijgt naar de sever volgens het protocol.
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
     * Deze methode zorgt ervoor dat je gegevens kunt ontvangen die je van de server krijgt.
     * Er wordt eerst gekeken hoelang het bericht gaat zijn en haalt vervolgens deze gegevens op.
     */
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