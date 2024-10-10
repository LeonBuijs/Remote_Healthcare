namespace Server;

/**
 * Dataklasse om verbindingen, namen en sessies bij te houden
 */
public class ClientConnection(string name, Connection connection)
{
    public string Name { get; set; } = name;
    public Connection Connection { get; set; } = connection;
    public bool InSession { get; set; } = false;
    public string SessionTime { get; set; } = "";
    public string LiveData { get; set; } = "";
}