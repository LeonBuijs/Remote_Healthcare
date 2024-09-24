using System.Net.Sockets;

namespace Arts;

class Program
{
    private static TcpClient artsClient;
    private static NetworkStream artsStream;
    private static byte[] artsBuffer = new byte[128];
    private static string totalBuffer;

    public static void Main(string[] args)
    {
        artsClient = new TcpClient();
        //todo verander de host en poortnummer
        artsClient.BeginConnect("127.0.0.1", 8080, new AsyncCallback(OnConnect), null);
    }

    /**
     * Methode om het zoeken naar verbinding te stoppen
     * Gebeurt bij gevonden verbinding en start een listener methode
     */
    private static void OnConnect(IAsyncResult ar)
    {
        artsClient.EndConnect(ar);
        Console.WriteLine("Verbonden!");
        artsStream = artsClient.GetStream();
        artsStream.BeginRead(artsBuffer, 0, artsBuffer.Length, new AsyncCallback(OnRead), null);

        //todo write methode maken
    }

    /**
     * Het uitlezen van data die het vervolgens naar een methode stuurt om te verwerken
     */
    private static void OnRead(IAsyncResult ar)
    {
        int receivedBytes = artsStream.EndRead(ar);
        string receivedText = System.Text.Encoding.ASCII.GetString(artsBuffer, 0, receivedBytes);
        totalBuffer += receivedText;

        //todo: pakket opsplitsen
        //HandleData(packet);

        artsStream.BeginRead(artsBuffer, 0, artsBuffer.Length, new AsyncCallback(OnRead), null);
    }

    /**
     * Methode die gebruik maakt van ons protocol om de inkomende data af te handelen
     * 0: inloggegevens bevestigen
     * 1: realtime data verwerken
     * 2: lijst met actieve sessies tonen
     * 3: lijst met opgeslagen data per sessie
     */
    private static void HandleData(string[] packetData)
    {
        int packetPage = int.Parse(packetData[0]);
        switch (packetPage)
        {
            case 0:
                string argument = packetData[1];
                if (argument == "0")
                {
                    Console.WriteLine("Unknown user or password!");
                    //todo afhandelen geweigerd
                }
                else if (argument == "1")
                {
                    Console.WriteLine("Logged in!");
                    //todo afhandelen geaccepteerd
                }
                else
                {
                    Console.WriteLine("Something went wrong!");
                    //todo afhandelen overige fouten
                }

                break;
            case 1:
                //todo koppelen met GUI
                string speed = packetData[1];
                string distance = packetData[2];
                string power = packetData[3];
                string RPM = packetData[4];
                string heartbeat = packetData[5];

                Console.WriteLine($"received packet page: {packetPage} with {speed} speed, {distance} distance, " +
                                  $"{power} power, {RPM} RPM, {heartbeat} heartbeat");
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                Console.WriteLine("Unknown Packet Page");
                //todo better error handling
                break;
        }
    }
}