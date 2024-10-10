using System.Net.Sockets;
using System.Text;

/*
 * Klasse voor het testen van de server voor de arts/client
 * Het is mogelijk om met deze klasse handmatig alle commands te sturen
 */

Console.WriteLine("Voer een port in voor arts of client (6666/7777)");
var port = Console.ReadLine();

var client = new TcpClient();

if (port != null) client.Connect("127.0.0.1", Int32.Parse(port));

Console.WriteLine("typ commands om naar de server te sturen");

var buffer = new byte[128];

Thread tReceive = new Thread(() =>
{
    while (true)
    {
        var received = client.GetStream().Read(buffer, 0, buffer.Length);

        Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, received));
    }
});
tReceive.Start();

Thread tSend = new Thread(start: () =>
{
    while (true)
    {
        var message = Console.ReadLine();

        if (message != null) client.GetStream().Write(Encoding.ASCII.GetBytes(message));
        client.GetStream().Flush();
    
    }
});
tSend.Start();

