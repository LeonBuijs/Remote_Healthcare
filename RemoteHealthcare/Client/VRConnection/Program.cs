using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
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
        // string uuidBike = CreateNodeForBike(stream);
        // string uuidTerrain = CreateNodeForTerrain(stream);

        string uuidRoute = CreateRoute(stream);


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

    private static string CreateRoute(NetworkStream stream)
    {
        SendPacket(stream, "{\"id\" :" +
                           "\"tunnel/send\", " +
                           "\"data\" :" +
                           "{\"dest\" :\"" +
                           SessionID +
                           "\", " +
                           "\"data\" :" +
                           "{\"id\" : \"route/add\", " +
                           "\"data\" :{ \"nodes\" : " +
                           "[" +
                           "{\"pos\" : [ 0, 0, 0  ],\"dir\" : [ 5, 0, -5]}," +
                           "{\"pos\" : [ 50, 0, 0  ],\"dir\" : [ 5, 0, 5]}," +
                           "{\"pos\" : [ 50, 0, 50  ],\"dir\" : [ -5, 0, 5]}," +
                           "{\"pos\" : [ 0, 0, 50  ],\"dir\" : [ -5, 0, -5]} " +
                           "]" +
                           "}}}}");
        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        return jsonObject["data"]["data"]["data"]["uuid"].ToString();
    }

    private static void createTerrain(NetworkStream stream)
    {
        int[] newArray = new int[256 * 256];

        for (int i = 0; i < newArray.Length; i++)
        {
            newArray[i] = 1;
        }

        StringBuilder jsonBuilder = new StringBuilder();
        jsonBuilder.Append("[");

        for (int i = 0; i < newArray.Length; i++)
        {
            jsonBuilder.Append(newArray[i]);

            // Voeg een komma toe als dit niet het laatste element is
            if (i < newArray.Length - 1)
            {
                jsonBuilder.Append(",");
            }
        }

        jsonBuilder.Append("]");

        // De resulterende JSON-string
        string jsonString = jsonBuilder.ToString();

        SendPacket(stream, "{\"id\" : " +
                           "\"tunnel/send\", " +
                           "\"data\" :" +
                           "{\"dest\" : \"" +
                           SessionID +
                           "\", " +
                           "\"data\" :" +
                           "{\"id\" : \"scene/terrain/add\", " +
                           "\"data\" : " +
                           "{\"size\" : [ 3, 3 ], " +
                           "\"heights\" : " + jsonString +
                           "}}}}");
        RecievePacket(stream);
    }

    private static string CreateNodeForBike(NetworkStream stream)
    {
        SendPacket(stream, "{\"id\" :" +
                           "\"tunnel/send\", " +
                           "\"data\" :" +
                           "{\"dest\" :\"" +
                           SessionID +
                           "\", " +
                           "\"data\" :" +
                           "{\"id\" : \"scene/node/add\", " +
                           "\"data\" :" +
                           "{\"name\" : \"test\", " +
                           "\"components\" : {" +
                           "\"transform\" : " +
                           "{\"position\" : [ 0, 0, 0 ], " +
                           "\"scale\" : 1, " +
                           "\"rotation\" : [ 0, 0, 0 ]}, " +
                           "\"model\" : " +
                           "{\"file\" : \"data/NetworkEngine/models/bike/bike.fbx\"}, " +
                           "}}}}}");
        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        return jsonObject["data"]["data"]["data"]["uuid"].ToString();
    }

    private static string CreateNodeForTerrain(NetworkStream stream)
    {
        createTerrain(stream);
        SendPacket(stream, "{\"id\" :" +
                           "\"tunnel/send\", " +
                           "\"data\" :" +
                           "{\"dest\" :\"" +
                           SessionID +
                           "\", " +
                           "\"data\" :" +
                           "{\"id\" : \"scene/node/add\", " +
                           "\"data\" :" +
                           "{\"name\" : \"test\", " +
                           "\"components\" : {" +
                           "\"transform\" : " +
                           "{\"position\" : [ 0, 0, 0 ], " +
                           "\"scale\" : 1, " +
                           "\"rotation\" : [ 0, 0, 0 ]}, " +
                           "\"terrain\" : " +
                           "{\"smoothnormals\" : true}" +
                           "}}}}}");
        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        return jsonObject["data"]["data"]["data"]["uuid"].ToString();
    }

    private static void SetTime(NetworkStream stream, int time)
    {
        SendPacket(stream, "{\"id\" : " +
                           "\"tunnel/send\", " +
                           "\"data\" :" +
                           "{\"dest\" : \"" +
                           SessionID +
                           "\", " +
                           "\"data\" :" +
                           "{\"id\" : \"scene/skybox/settime\", " +
                           "\"data\" : " +
                           "{\"time\" : " + time + "}}}}");
        RecievePacket(stream);
    }

    private static void CreateTunnel(NetworkStream stream)
    {
        // Stap 2
        SendPacket(stream, "{\"id\" : \"session/list\"}");

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
        SendPacket(stream, "{\"id\" : " +
                           "\"tunnel/create\", " +
                           "\"data\" : " +
                           "{\"session\" : \"" +
                           ID +
                           "\",}}");

        jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        SessionID = jsonObject["data"]["id"].ToString();

        // Stap 5
        SendPacket(stream, "{\"id\" : " +
                           "\"tunnel/send\", " +
                           "\"data\" :" +
                           "{\"dest\" : \"" +
                           SessionID +
                           "\", " +
                           "\"data\" :" +
                           "{\"id\" : \"scene/reset\", " +
                           "\"data\" : {}}}}");
        RecievePacket(stream);
    }

    private static void SendPacket(NetworkStream stream, string packetString)
    {
        var array = Encoding.ASCII.GetBytes(packetString);
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