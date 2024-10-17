namespace Server;

public interface IDoctorCallback
{
    public void OnReceivedMessage(string message, Connection connection);
}