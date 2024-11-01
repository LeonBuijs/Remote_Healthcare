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

    public Connection(string ipAddress, int port, MessageHandler messageHandler)
    {
        // Verbind met server
        client = new TcpClient(ipAddress, port);
        stream = client.GetStream();

        this.messageHandler = messageHandler;

        StartThreadReceive();
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
                var buffer = new byte[1024];
                var bytesRead = stream.Read(buffer, 0, buffer.Length);

                try
                {
                    var received = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    received = received.Trim('\n');
                    messageHandler.ProcessMessage(received);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (e.Equals(new InvalidOperationException()))
                    {
                        SendMessage("404");
                        messageHandler.Disconnect();
                        // Disconnect();
                        return; 
                    }

                    messageHandler.Disconnect();
                    // Disconnect();
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