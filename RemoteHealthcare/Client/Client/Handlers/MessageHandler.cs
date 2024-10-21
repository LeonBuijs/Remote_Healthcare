using System;
using System.Text;
using Client;
using Client.Handlers;

namespace ClientGUI;

public class MessageHandler : IBLECallback
{
    private BLEHandler bleHandler;
    private VRHandler vrHandler;
    public bool loggedIn { get; set; }

    public MessageHandler(BLEHandler bleHandler)
    {
        this.bleHandler = bleHandler;
        // this.vrHandler = vrHandler; // TODO deze initten als de doctor een sessie start
    }
    
    /**
     * Methode om inkomende berichten te verwerken
     */
    public void ProcessMessage(string message)
    {
        Console.WriteLine($"Received message: {message}");
        
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var identifier = message[0];
        var content = message.Substring(2);

        switch (identifier)
        {
            case '0':
                HandleChatMessage(content);
                break;
            case '1':
                HandleBikeResistanceSettings(content);
                break;
            case '2':
                HandleStartCommand();
                break;
            case '3':
                HandleStopCommand();
                break;
            case '4':
                HandleEmergencyStopCommand();
                break;
            case '5':
                HandleLoginConfirmation(content);
                break;
        }
    }

    /**
     * Helper methode om een chatbericht door te sturen naar de vr-omgeving
     */
    private void HandleChatMessage(string message)
    {
        Console.WriteLine($"Chat message from doctor: {message}");
        vrHandler.SendChatToVr(message);
    }

    /**
     * Helper methode om aan de hand van een commando de weerstand van een fiets in te stellen
     */
    private void HandleBikeResistanceSettings(string settings)
    {
        Console.WriteLine($"Bike resistance settings: {settings}");

        var resistance = Encoding.UTF8.GetBytes(settings);
        // TODO: kijken of dit echt werkt
        bleHandler.SetResistance(resistance[0]);
    }

    /**
     * Helper methode om een startcommando uit te voeren
     */
    private void HandleStartCommand()
    {
        vrHandler.StartSession();
    }

    /**
     * Helper methode om een stopcommando uit te voeren
     */
    private void HandleStopCommand()
    {
        vrHandler.StopSession();
    }

    /**
     * Helper methode om een emergency-stop commando uit te voeren
     */
    private void HandleEmergencyStopCommand()
    {
        vrHandler.EmergencyStop();
    }

    /**
     * Helper methode om de login-bevestiging te verwerken
     */
    private void HandleLoginConfirmation(string confirmation)
    {
        if (confirmation == "1")
        {
            Console.WriteLine("Logging in");
            loggedIn = true;
        }
        else
        {
            Console.WriteLine("Wrong username or password");
            loggedIn = false;
        }
    }

    /**
     * Methode om alles netjes af te sluiten
     */
    public void Disconnect()
    {
        bleHandler.Disconnect();
    }
    
    public void OnReceivedBikeData(BikeData bikeData)
    {
        // TODO   
        Console.WriteLine(
            $"Speed: {bikeData.Speed} RPM: {bikeData.Rpm} Distance: {bikeData.Distance} Watts: {bikeData.Watt} Time: {bikeData.Time} HeartRate: {bikeData.HeartRate}");
    }
}