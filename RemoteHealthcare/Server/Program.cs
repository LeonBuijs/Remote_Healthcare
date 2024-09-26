namespace Server;

public class Server : IArtsCallback, IClientCallback
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting server...");
        Server server = new Server();
        server.setCallbacks();
    }

    private void setCallbacks()
    {
        ConnectionHandler handler = new ConnectionHandler(this, this);
        handler.Start();
    }
    /**
     * Callbacks voor het verwerken van requests vanuit de Arts en Client(s)
     */
    void IArtsCallback.OnReceivedMessage(string message, Connection connection)
    {
        //todo requests verwerken en resultaat terugsturen
        Console.WriteLine("Arts: " + message.Replace("-", " "));
        
        connection.Send("hallo terug");
    }

    void IClientCallback.OnReceivedMessage(string message, Connection connection)
    {
        //todo requests verwerken en resultaat terugsturen
        Console.WriteLine("Client: " + message);
    }
}