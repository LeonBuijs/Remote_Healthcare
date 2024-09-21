using System.Net.Sockets;
using System.Text;

namespace Server;

public class Connection
{
    private string ip;
    private int port;
    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer;

    public Connection(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
        client = new TcpClient(this.ip, this.port);
        stream = client.GetStream();
        buffer = new byte[1024];
    }

    public void Send(string msg)
    {
        byte[] array = Encoding.ASCII.GetBytes(msg);
        stream.Write(array, 0, array.Length);   
    }

    public string Receive()
    {
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        return BitConverter.ToString(buffer, 0, bytesRead);
    }
    
}