using System;

namespace Client.Handlers;

public class MessageHandler : IBLECallback
{
    public BLEHandler BleHandler;
    public VRHandler VrHandler { get; set; }
    public bool LoggedIn;

    public MessageHandler()
    {
        BleHandler = new BLEHandler(this);
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

        switch (identifier)
        {
            case '0':
                HandleChatMessage(message.Substring(2));
                break;
            case '1':
                HandleBikeResistanceSettings(message.Substring(2));
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
                HandleLoginConfirmation(message.Substring(2));
                break;
        }
    }

    /**
     * Helper methode om een chatbericht door te sturen naar de vr-omgeving
     */
    private void HandleChatMessage(string message)
    {
        Console.WriteLine($"Chat message from doctor: {message}");
        VrHandler.SendChatToVr(message);
    }

    /**
     * Helper methode om aan de hand van een commando de weerstand van een fiets in te stellen
     */
    private void HandleBikeResistanceSettings(string settings)
    {
        Console.WriteLine($"Bike resistance settings: {settings}");
        
        var resistance = int.Parse(settings);
        resistance *= 2;
        
        // TODO: kijken of dit echt werkt
        BleHandler.SetResistance(resistance);
    }

    /**
     * Helper methode om een startcommando uit te voeren
     */
    private void HandleStartCommand()
    {
        VrHandler.StartSession(); 
    }

    /**
     * Helper methode om een stopcommando uit te voeren
     */
    private void HandleStopCommand()
    {
        VrHandler.StopSession();
    }

    /**
     * Helper methode om een emergency-stop commando uit te voeren
     */
    private void HandleEmergencyStopCommand()
    {
        VrHandler.EmergencyStop();
    }

    /**
     * Helper methode om de login-bevestiging te verwerken
     */
    private void HandleLoginConfirmation(string confirmation)
    {
        if (confirmation == "1")
        {
            Console.WriteLine("Logging in");
            LoggedIn = true;
        }
        else
        {
            Console.WriteLine("Wrong username or password");
            LoggedIn = false;
        }
    }

    /**
     * Methode om alles netjes af te sluiten
     */
    public void Disconnect()
    {
        BleHandler.Disconnect();
    }

    public void OnReceivedBikeData(BikeData bikeData)
    {
        VrHandler.SendBikeDataToVr(bikeData);
        
        Console.WriteLine(bikeData);
    }
}