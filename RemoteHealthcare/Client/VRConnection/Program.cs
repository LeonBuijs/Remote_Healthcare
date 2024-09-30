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
        NetworkStream stream = tcpClient.GetStream();
        Console.WriteLine("Verbonden met de server\n");

        CreateTunnel(stream);
        
        SetTime(stream, 0);
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
        Console.WriteLine(BitConverter.ToString(combinedArray));
        Console.WriteLine("Bericht verstuurd\n");
    }

    private static string RecievePacket(NetworkStream stream)
    {
        var buffer = new byte[2048];
        var bytesRead = stream.Read(buffer, 0, buffer.Length);

        int bufferLength = buffer[0];
        Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, bytesRead));
        
        Console.WriteLine("Bericht ontvangen\n");
        return Encoding.ASCII.GetString(buffer, 0, bytesRead);
    }
}