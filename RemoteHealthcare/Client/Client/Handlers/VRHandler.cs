using System.Net.Sockets;

namespace Client;

/**
 * Dummy klasse waarmee berichten naar de VR-Server gestuurd kunnen worden
 */
public class VRHandler
{
    private bool inSession;
    private TcpClient tcpClient;

    public VRHandler()
    {
        ConnectToVrConnection();
    }

    public void SendBikeDataToVr(int speed)
    {
        //todo
    }

    public void SendChatToVr(string chat)
    {
        //todo
    }

    public void StartSession()
    {
        //todo
        inSession = true;
    }

    public void StopSession()
    {
        //todo
        inSession = false;
    }

    public void EmergencyStop()
    {
        //todo
        inSession = false;
    }

    /**
     * Methode om een TCP-verbinding op localhost op te zetten om met de VRConnection applicatie te praten
     */
    private void ConnectToVrConnection()
    {
        tcpClient = new TcpClient();
        tcpClient.Connect("127.0.0.1", 9999);
    }
}