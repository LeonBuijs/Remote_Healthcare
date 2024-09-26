using System.Net.Sockets;
using System.Text;

namespace Server;

/**
 * Klasse die de TCP-verbinding regelt voor de artsen en clients
 */
public class Connection
{
    public NetworkStream stream;

    public Connection(TcpClient client)
    {
        stream = client.GetStream();
    }

    public void Send(string msg)
    {
        var array = Encoding.ASCII.GetBytes(msg);
        stream.Write(array, 0, array.Length);
    }

    public string Receive()
    {
        var buffer = new byte[1024];
        var bytesRead = stream.Read(buffer, 0, buffer.Length);
        return Encoding.ASCII.GetString(buffer, 0, bytesRead);
    }
}