using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ClientGUI;

namespace Client;

/**
 * Klasse voor het ontvangen en verzenden van data naar/van de server
 */
public class Connection
{
    private TcpClient client;
    private NetworkStream stream;
    private MessageHandler messageHandler;
        
    public Connection(string ipAddress, int port, MessageHandler messageHandler)
    {
        // Verbind met server
        client = new TcpClient(ipAddress, port);
        stream = client.GetStream();
        
        this.messageHandler = messageHandler;
            
        StartThreadReceive();
    }

    // TODO: wanneer verbinding met de server verloren wordt, netjes afsluiten
    /**
     * Methode om de thread te starten die alle berichten ontvangt
     */
    private void StartThreadReceive()
    {
        var threadReceive = new Thread(() =>
        {
            while (true)
            {
                var buffer = new byte[1024];
                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                var received=  Encoding.ASCII.GetString(buffer, 0, bytesRead);
                
                messageHandler.ProcessMessage(received);
            }
        });
        threadReceive.Start();
    }
    

    public void SendMessage(string message)
    {
        var data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }
        
    public void CloseConnection()
    {
        stream.Close();
        client.Close();
        Console.WriteLine("Verbinding gesloten.");
    }
}