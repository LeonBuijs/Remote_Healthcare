using System.Net.Sockets;
using System.Text;

class VRConnection
{
    public static void Main(string[] args)
    {
        TcpClient tcpClient = new TcpClient();
        tcpClient.Connect("85.145.62.130", 6666);
        NetworkStream stream = tcpClient.GetStream();
        Console.WriteLine("Verbonden met de server");

        var array = Encoding.ASCII.GetBytes("{\"id\" : \"session/list\"}");
        byte[] prefix = new byte[] { (byte)array.Length, 0x00, 0x00, 0x00 };
        byte[] combinedArray = new byte[prefix.Length + array.Length];
        Array.Copy(prefix, 0, combinedArray, 0, prefix.Length);
        Array.Copy(array, 0, combinedArray, prefix.Length, array.Length);
        stream.Write(combinedArray, 0, combinedArray.Length);
        Console.WriteLine(BitConverter.ToString(combinedArray));
        Console.WriteLine("Bericht verstuurd");

        var buffer = new byte[1024];
        var bytesRead = stream.Read(buffer, 0, buffer.Length);
        Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, bytesRead));
        
        Console.WriteLine("Bericht ontvangen");

    }
}