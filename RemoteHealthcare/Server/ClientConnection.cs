namespace Server;

/**
 * Dataklasse om verbindingen, namen en sessies bij te houden
 */
public class ClientConnection(string name, Connection connection)
{
    public Connection Connection { get; set; }
    public string Name { get; } = name;
    public bool InSession { get; set; }
    public string SessionTime { get; set; } = "";
    public string LiveData { get; set; } = "0 0 0 0 0 0";
}