using System;
using System.Text;

namespace Client.Handlers;

public class MessageHandler : IBLECallback
{
    private BLEHandler bleHandler;
    public VRHandler VrHandler { get; set; }
    public bool LoggedIn { get; set; }

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
        VrHandler.SendChatToVr(message);
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
        bleHandler.Disconnect();
    }

    public void OnReceivedBikeData(BikeData bikeData)
    {
        VrHandler.SendBikeDataToVr(bikeData);
        Console.WriteLine(bikeData);
    }
}