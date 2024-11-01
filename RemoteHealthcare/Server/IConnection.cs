namespace Server;

public interface IConnection
{
    bool Access { get; set; }
    void Send(string msg);
    string Receive();
}