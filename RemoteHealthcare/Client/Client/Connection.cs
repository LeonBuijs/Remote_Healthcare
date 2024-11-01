using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Client.Handlers;

namespace Client;

/**
 * Klasse voor het ontvangen en verzenden van data naar/van de server
 */
public class Connection
{
    private TcpClient client;
    private NetworkStream stream;
    private MessageHandler messageHandler;

    private bool isEncrypted = false;
    //encryption keys
    private string privateKey;
    public string publicKey { get; set; }
    public string PublicKeyServer { get; set; }

    public Connection(string ipAddress, int port, MessageHandler messageHandler)
    {
        if (port == 6666)
        {
            Console.WriteLine($"Encrypted activated on port: {port}");
            isEncrypted = true;
        }
        // Verbind met server
        client = new TcpClient(ipAddress, port);
        stream = client.GetStream();
        
        if (isEncrypted)
        {
            SetRSAKeys();
            SendMessage(publicKey);
        }
        
        this.messageHandler = messageHandler;
        StartThreadReceive();
    }
    
    private void SetRSAKeys()
    {
        var (generatedPublicKey, generatedPrivateKey) = Encryption.GenerateRsaKeyPair();
        publicKey = generatedPublicKey;
        privateKey = generatedPrivateKey;
    }

    /**
     * Methode om de thread te starten die alle berichten ontvangt
     */
    private void StartThreadReceive()
    {
        var threadReceive = new Thread(() =>
        {
            while (true)
            {
                try
                {
                    var buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var result = new byte[bytesRead];
                    Array.Copy(buffer, result, bytesRead);
                    
                    if (PublicKeyServer == null && isEncrypted)
                    {
                        PublicKeyServer = Encoding.ASCII.GetString(result);
                        continue;
                    }

                    string received;
                    
                    if (isEncrypted)
                    {
                        received = Encryption.DecryptData(result, privateKey);
                        Console.WriteLine($"Server received decrypted message:\n{received}");
                    }
                    else
                    {
                        
                        received = Encoding.ASCII.GetString(result);
                    }
                    
                    messageHandler.ProcessMessage(received);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (e.Equals(new InvalidOperationException()))
                    {
                        SendMessage("404");
                        messageHandler.Disconnect();
                        Disconnect();
                        return; 
                    }

                    messageHandler.Disconnect();
                    Disconnect();
                    return;
                }
            }
        });
        threadReceive.Start();
    }

    /**
     * Methode om een bericht naar de verbonden TCP-client te sturen
     */
    public void SendMessage(string message)
    {
        Console.WriteLine($"Sending message: {message}");
        var data = Encoding.ASCII.GetBytes(message);
        if (isEncrypted && PublicKeyServer != null)
        {
            data = Encryption.EncryptData(data, PublicKeyServer);
        }
        stream.Write(data, 0, data.Length);
    }

    /**
     * Methode om alle verbonden zaken af te sluiten
     */
    private void Disconnect()
    {
        stream.Close();
        client.Close();
        Console.WriteLine("Verbinding gesloten.");
    }
}