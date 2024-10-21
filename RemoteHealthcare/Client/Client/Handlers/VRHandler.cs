using System.Net.Sockets;

namespace Client;

/**
 * Dummy klasse waarmee berichten naar de VR-Server gestuurd kunnen worden
 */
public class VRHandler
{
    private bool inSession;
    private Connection connection;

    public VRHandler(Connection connection)
    {
        this.connection = connection;
    }

    public void SendChatToVr(string chat)
    {
        //todo
        connection.SendMessage($"0 {chat}");
    }

    public void SendBikeDataToVr(BikeData bikeData)
    {
        if (inSession)
        {
            connection.SendMessage($"1 {bikeData}");

        }
    }

    public void StartSession()
    {
        connection.SendMessage("2");
        inSession = true;
    }

    public void StopSession()
    {
        connection.SendMessage("3");
        inSession = false;
    }

    public void EmergencyStop()
    {
        connection.SendMessage("4");
        inSession = false;
    }

    public void SendNameToVr(string firstName, string lastName)
    {
        connection.SendMessage($"5 {firstName} {lastName}");
    }
}