namespace Server;

public interface IArtsCallback
{
    public void OnReceivedMessage(string message, Connection connection);
}