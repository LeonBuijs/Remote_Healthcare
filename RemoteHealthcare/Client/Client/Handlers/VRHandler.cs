namespace Client;

/**
 * Dummy klasse waarmee berichten naar de VR-Server gestuurd kunnen worden
 */
public class VRHandler
{
    private bool inSession;

    public void SendSpeedToVr(int speed)
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
}