using System.Collections;
using Server;
 
public class ServerSimulation():IDoctorCallback, IClientCallback
{
    private static List<TestClient> clients = [];
    public static void Main(string[] args)
    {
        clients.Add(new TestClient("Frodo", "Baggins", "20022003"));
        clients.Add(new TestClient("Biba", "Beer", "03071987"));
        clients.Add(new TestClient("jaap", "ZonderBroek", "01121960"));
        clients.Add(new TestClient("jasmien", "Waterpad", "19052012"));
        ServerSimulation simulation = new ServerSimulation();
        simulation.SetCallbacks();
    }

    public static void Start()
    {
        
    }

    private void SetCallbacks()
    {
        ConnectionHandler handler = new ConnectionHandler(this, this);
        handler.Start();
    }

    void IDoctorCallback.OnReceivedMessage(string message, Connection connection)
    {
        var messageParts = message.Split(' ');
        ArtsCallbackHandler(connection, messageParts);
    }
    
    private Boolean CheckLogin(string username, string password)
    {
        return username == "admin" && password == "admin";
    }
    
    private void ArtsCallbackHandler(Connection connection, string[] messageParts)
    {
        switch (Int32.Parse(messageParts[0]))
        {
            case 0:
                if (CheckLogin(messageParts[1], messageParts[2]))
                {
                    connection.Send("0 1");
                }
                else
                {
                    connection.Send("0 0");
                }
                break;
            case 1:
                Console.WriteLine("start commando gestuurd naar" + messageParts[1]);
                break;
            case 2:
                Console.WriteLine("stop commando gestuurd naar" + messageParts[1]);
                break;
            case 3:
                // Stuur een noodstopcommando naar een specifieke client
                Console.WriteLine("noodstopstop commando gestuurd naar" + messageParts[1]);
                break;
            case 4:
                // Stuur een bericht naar een specifieke client
                Console.WriteLine(messageParts[2] + ", gestuurd naar" + messageParts[1]);
                break;
            case 5:
                connection.Send($"5 {messageParts[1]}");
                Console.WriteLine(messageParts[1] + ", verstuurd naar iedereen");
                break;
            case 6:
                // Stuur de weerstand naar een specifieke client
                Console.WriteLine(messageParts[2] + ", weerstand verstuurd naar :" + messageParts[1]);
                break;
            case 7:
                foreach (var client in clients)
                {
                    if (client.voornaam == messageParts[1] && client.achternaam == messageParts[2] && client.geboortedatum == messageParts[3])
                    {
                        ArrayList temp = client.GetHistory();
                        foreach (var data in temp)
                        {
                            connection.Send($"3 {client.GetClientInfo()} {data}");
                            Thread.Sleep(200);
                        }
                    }
                }
                break;
            case 8:
                // clients.ForEach(client => connection.Send($"2 {client.GetClientInfo()}"));
                foreach (var client in clients)
                {
                    connection.Send($"2 {client.GetClientInfo()}");
                    Console.WriteLine(client.GetClientInfo());
                    Thread.Sleep(500);
                }
                break;
            case 9:
                clients.Add(new TestClient(messageParts[1], messageParts[2], messageParts[3]));
                break;
            case 10:
                foreach (var testClient in clients)
                {
                    if (testClient.voornaam == messageParts[1] && testClient.achternaam == messageParts[2] && testClient.geboortedatum == messageParts[3])
                    {
                        connection.Send($"1 {testClient.GetClientInfo()} {testClient.GetData()}");
                    }
                }
                break;
        }
        
    }
    
    void IClientCallback.OnReceivedMessage(string message, Connection connection)
    {
        
    }
}

public class TestClient()
{
    public string voornaam { get; }
    public string achternaam { get; }
    public string geboortedatum { get; }
    private int speed = 0;
    private int hardrate = 0;
    private int rpm = 0;
    private int distance = 0;
    private int power = 0;
    private int tijd = 0;
    private int extra = 0;
    private ArrayList history = new ArrayList();
    private int count = 0;
    public TestClient(string voornaam, string achternaam, string geboortedatum) : this()
    {
        this.voornaam = voornaam;
        this.achternaam = achternaam;
        this.geboortedatum = geboortedatum;
        
    }

    public string GetData()
    {
        if (count == 5)
        {
            tijd++;
            history.Add(tijd + " " + speed + " " + hardrate + " " + rpm + " " + distance + " " + power + " " + extra);
            count = 0;
        }
        speed++; hardrate += 2; rpm+= 3; distance+= 4; power += 5; extra += 6; 
        count++;
        return speed + " " + hardrate + " " + rpm + " " + distance + " " + power + " " + extra;
    }

    public ArrayList GetHistory()
    {
        return history;
    }

    public string GetClientInfo()
    {
        return voornaam + " " + achternaam + " " + geboortedatum;
    }
}