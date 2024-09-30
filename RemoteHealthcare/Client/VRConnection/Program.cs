using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;

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
        // SetTime(stream, 0);
        
        
        SendPacket(stream, "{\"id\" :" +
                "\"tunnel/send\", " +
                "\"data\" :" +
                "{\"dest\" :\"" +
                SessionID +
                "\", " +
                "\"data\" :" +
                "{\"id\" : \"scene/node/add\", " +
                "\"data\" :" +
                "{\"name\" : \"test\" " +
                // "\"components\" :" +
                // "{\"transform\" : " +
                // "{\"position\" : [ 0, 0, 0 ], " +
                // "\"scale\" : 1, " +
                // "\"rotation\" : [ 0, 0, 0 ]}," +
                // "\"model\" : " +
                // "{\"file\" : \"filename\", " +
                // "\"cullbackfaces\" : true, " +
                // "\"animated\" : false, " +
                // "\"animation\" : \"animationname\"}," +
                // "\"terrain\" : " +
                // "{\"smoothnormals\" : true}, " +
                // "\"panel\" : " +
                // "{\"size\" : [ 1, 1 ], " +
                // "\"resolution\" : [ 512, 512 ], " +
                // "\"background\" : [ 1, 1, 1, 1], " +
                // "\"castShadow\" : true}, " +
                // "\"water\" :" +
                // "{\"size\" : [ 20, 20 ], " +
                // "\"resolution\" : 0.1}
                 "}}}}");
        
        
        
        RecievePacket(stream);
        RecievePacket(stream);
        
        // SendPacket(stream, "{\"id\" : " +
        //                    "\"tunnel/send\", " +
        //                    "\"data\" :" +
        //                    "{\"dest\" : \"" +
        //                    SessionID +
        //                    "\", " +
        //                    "\"data\" :" +
        //                    "{\"id\" : \"scene/panel/drawtext\", " +
        //                    "\"data\" : " +
        //                    "{\"id\" : {1}, " +
        //                    "\"text\" : \"Hello World\", " +
        //                    "\"position\" : [ 100.0, 100.0 ], " +
        //                    "\"size\" : 128.0, " +
        //                    "\"color\" : [ 0,0,0,1 ], }}}}");
        // RecievePacket(stream);
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
        RecievePacket(stream);
        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        string ID = jsonObject["data"][0]["id"].ToString();
        
        // Stap 4
        SendPacket(stream, "{\"id\" : " +
                           "\"tunnel/create\", " +
                           "\"data\" : " +
                           "{\"session\" : \"" +
                           ID +
                           "\",}}");
        
        RecievePacket(stream);
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
        RecievePacket(stream);
    }

    private static void SendPacket(NetworkStream stream, string packetString)
    {
        var array = Encoding.ASCII.GetBytes(packetString);
        byte[] prefix = new byte[] { (byte)array.Length, 0x00, 0x00, 0x00 };
        byte[] combinedArray = new byte[prefix.Length + array.Length];
        Array.Copy(prefix, 0, combinedArray, 0, prefix.Length);
        Array.Copy(array, 0, combinedArray, prefix.Length, array.Length);
        stream.Write(combinedArray, 0, combinedArray.Length);
        Console.WriteLine(Encoding.ASCII.GetString(combinedArray, 0, combinedArray.Length));
        Console.WriteLine("Bericht verstuurd\n");
    }

    private static string RecievePacket(NetworkStream stream)
    {
        try
        {
            var buffer = new byte[2048];
            var bytesRead = stream.Read(buffer, 0, buffer.Length);

            int bufferLength = buffer[0];

            if (Encoding.ASCII.GetString(buffer, 0, bytesRead).StartsWith("{"))
            {
                Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, bytesRead));
                Console.WriteLine("Bericht ontvangen\n");
                return Encoding.ASCII.GetString(buffer, 0, bytesRead);

            }

            if (Encoding.ASCII.GetString(buffer, 0, bytesRead).Length < 1)
            {
                Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, bytesRead));
                Console.WriteLine("Bericht ontvangen\n");
                return Encoding.ASCII.GetString(buffer, 0, bytesRead);
            }

            string returnString = Encoding.ASCII.GetString(buffer, 0, bytesRead).Substring(1);

            Console.WriteLine(returnString);
            Console.WriteLine("Bericht ontvangen\n");

            return returnString;
        }
        catch (IOException e)
        {
            Console.WriteLine("Geen reactie...");
            return "";
        }
    }
}