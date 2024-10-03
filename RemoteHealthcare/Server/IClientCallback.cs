namespace Server;

public interface IClientCallback
{
    public void OnReceivedMessage(string message, Connection connection);
}