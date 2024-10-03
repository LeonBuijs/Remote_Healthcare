namespace Server;

/**
 * Dataklasse om verbindingen, namen en sessies bij te houden
 */
public class ClientConnection(string name)
{
    public string name { get; set; } = name;
    public bool inSession { get; set; } = false;
    public string sessionTime { get; set; } = "";
    public string liveData { get; set; } = "";
}