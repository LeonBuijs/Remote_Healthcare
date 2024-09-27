using System.Net.Sockets;
using System.Text;

class VRConnection
{
    public static void Main(string[] args)
    {
        // Stap 1
        TcpClient tcpClient = new TcpClient();
        tcpClient.Connect("85.145.62.130", 6666);
        NetworkStream stream = tcpClient.GetStream();
        Console.WriteLine("Verbonden met de server\n");

        // Stap 2
        SendPacket(stream, "{\"id\" : \"session/list\"}");

        // Stap 3
        RecievePacket(stream);
        
        // Stap 4 TODO: verder uitwerken
        SendPacket(stream, "{\"id\" : " +
                           "\"tunnel/create\", " +
                           "\"data\" : " +
                           "{\"session\" : \"c22ad75b-a340-4ac2-9e63-d8cb164b6c5f\",}}");
        
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

    private static void RecievePacket(NetworkStream stream)
    {
        var buffer = new byte[2048];
        var bytesRead = stream.Read(buffer, 0, buffer.Length);
        Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, bytesRead));
        
        Console.WriteLine("Bericht ontvangen\n");
    }
}