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

    private int setRSAKeyTries = 0;
    /**
     * <summary>
     * Benaderd Encryptie klasse van de library
     * Vervolgens wordt er een tuple terug geleverd met public- en privateKey
     * Als er keys null zijn, zal hij recursief nieuwe keys proberen te maken.
     * Na 5 pogingen zal hij een exception throwen
     * </summary>
     */
    private void SetRSAKeys()
    {
        if (setRSAKeyTries >= 5)
        {
            throw new Exception("Failed to set RSA key in 5 tries.");
        }
        var (publicKey, privateKey) = Encryption.GenerateRsaKeyPair();
        PublicKeyServerClient = publicKey;
        privateKeyServer = privateKey;
        if (string.IsNullOrEmpty(PublicKeyServerClient) || string.IsNullOrEmpty(privateKeyServer))
        {
            setRSAKeyTries++;
            SetRSAKeys();
        }
    }

    /**
     * <summary>
     * Methode die een bericht over de socket stuurt
     * Encrypt altijd tenzij er false meegegeven wordt
     * </summary>
     * <param name="msg">Het bericht wat overgestuurd wordt</param>
     * <param name="encryption">Staat default true, wanneer false geen encryptie</param>
     */
    public void Send(string msg, bool encryption = true)
    {
        var array = Encoding.ASCII.GetBytes(msg + "\n");
        Console.WriteLine($"Sending message: {msg}");
        if (encryption)
        {
            array = Encryption.EncryptData(array, publicKeyConnection);
            //Vang het mogelijke null resultaat af
            if (array == null)
            {
                Console.WriteLine($"Encryption failed, incorrect data. Aborting send");
                return;
            }
        }
        
        Stream.Write(array, 0, array.Length);
    }

    /**
     * <summary>
     * Methode die het data ontvangt
     * Het eerste bericht zal ALTIJD de key zijn
     * Daarom als de key false is, wordt ontvangen data als key neergezet
     * </summary>
     * <returns>String - Gedecodeerde ontvangen data</returns>
     */
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
        if (string.IsNullOrEmpty(received))
        {
            Console.WriteLine($"Decryption failed, incorrect data. Aborting receive");
            return "69420";
        }
        Console.WriteLine($"Server received decrypted message:\n{received}");

        return received;
    }
}