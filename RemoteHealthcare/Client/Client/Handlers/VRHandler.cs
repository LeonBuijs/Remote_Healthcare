namespace Client.Handlers;

/**
 * Klasse waarmee berichten naar de VR-omgeving gestuurd kunnen worden
 */
public class VRHandler
{
    private bool inSession;
    private Connection connection;

    public VRHandler(Connection connection)
    {
        this.connection = connection;
    }

    /**
     * Methode om chats van de arts naar de VR-omgeving te sturen
     */
    public void SendChatToVr(string chat)
    {
        connection.SendMessage($"0 {chat}");
    }

    /**
     * Methode om data vanuit de fiets naar de VR-omgeving te sturen
     */
    public void SendBikeDataToVr(BikeData bikeData)
    {
        if (!inSession)// todo
        {
            connection.SendMessage($"1 {bikeData}");
        }
    }

    /**
     * Methode om een startcommando naar de VR-omgeving te sturen
     */
    public void StartSession()
    {
        connection.SendMessage("2");
        inSession = true;
    }

    /**
     * Methode om een stopcommando naar de VR-omgeving te sturen
     */
    public void StopSession()
    {
        connection.SendMessage("3");
        inSession = false;
    }

    /**
     * Methode om een noodstopcommando naar de VR-omgeving te sturen
     */
    public void EmergencyStop()
    {
        connection.SendMessage("4");
        inSession = false;
    }

    /**
     * Methode om de client naam naar de VR-omgeving te sturen
     */
    public void SendNameToVr(string firstName, string lastName)
    {
        connection.SendMessage($"5 {firstName} {lastName}");
    }
}