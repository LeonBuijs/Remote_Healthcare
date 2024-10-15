using System.Net.Sockets;
using System.Text;

namespace Server;

/**
 * Klasse die de TCP-verbinding regelt voor de artsen en clients
 */
public class Connection(TcpClient client)
{
    public readonly NetworkStream Stream = client.GetStream();
    public bool Access { get; set; }

    public void Send(string msg)
    {
        var array = Encoding.ASCII.GetBytes(msg);
        Stream.Write(array, 0, array.Length);
    }

    public string Receive()
    {
        var buffer = new byte[1024];
        var bytesRead = Stream.Read(buffer, 0, buffer.Length);
        return Encoding.ASCII.GetString(buffer, 0, bytesRead);
    }
}