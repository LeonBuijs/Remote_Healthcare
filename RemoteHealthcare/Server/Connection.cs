using System.Net.Sockets;
using System.Text;
using SecurityManager;

namespace Server;

/**
 * Klasse die de TCP-verbinding regelt voor de artsen en clients
 */
public class Connection
{
    public readonly NetworkStream Stream;
    public bool Access { set; get; }
    public string PublicKeyServerClient { set; get; }
    private string privateKeyServer;
    private string? publicKeyConnection;

    public Connection(TcpClient tcpClient)
    {
        Stream = tcpClient.GetStream();
        SetRSAKeys();
    }

    private void SetRSAKeys()
    {
        var (publicKey, privateKey) = Encryption.GenerateRsaKeyPair();
        PublicKeyServerClient = publicKey;
        privateKeyServer = privateKey;
    }

    public void Send(string msg, bool encryption = true)
    {
        var array = Encoding.ASCII.GetBytes(msg + "\n");
        Console.WriteLine($"Sending message: {msg}");
        if (encryption)
        {
            array = Encryption.EncryptData(array, publicKeyConnection);
        }
        
        Stream.Write(array, 0, array.Length);
    }

    public string Receive()
    {
        var buffer = new byte[1024];
        int bytesRead = Stream.Read(buffer, 0, buffer.Length);
        var result = new byte[bytesRead];
        Array.Copy(buffer, result, bytesRead);
        
        if (publicKeyConnection == null)
        {
            publicKeyConnection = Encoding.ASCII.GetString(result);
            return "69420";
        }
        
        var received = Encryption.DecryptData(result, privateKeyServer);
        Console.WriteLine($"Server received decrypted message:\n{received}");

        return received;
    }
}