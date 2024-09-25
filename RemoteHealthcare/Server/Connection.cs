using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

public class Connection
{
    private TcpListener listener;
    private TcpClient client;
    public NetworkStream stream;
    private byte[] buffer;

    public Connection(TcpClient client)
    {
        this.client = client;
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