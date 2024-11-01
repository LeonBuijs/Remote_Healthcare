namespace Server;

/**
 * Dataklasse om verbindingen, namen en sessies bij te houden
 */
public class ClientConnection(string name, IConnection connection)
{
    public string Name { get; } = name;
    public IConnection Connection { get; } = connection;
    public bool InSession { get; set; }
    public string SessionTime { get; set; } = "";
    public string LiveData { get; set; } = "0 0 0 0 0 0";
}