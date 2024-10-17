using System;
using System.Net.Sockets;
using System.Text;
using Client;

namespace ClientGUI;

public class MessageHandler
{
    private BLEHandler bleHandler;
    private VRHandler vrHandler;
    public bool loggedIn { get; set; }

    public MessageHandler(BLEHandler bleHandler, VRHandler vrHandler)
    {
        this.bleHandler = bleHandler;
        this.vrHandler = vrHandler;
    }


    public void ProcessMessage(string message)
    {
        Console.WriteLine($"Received message: {message}");
        
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var identifier = message[0];
        var content = message.Substring(1);

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

    private void HandleChatMessage(string message)
    {
        Console.WriteLine($"Chat message from doctor: {message}");
        vrHandler.SendChatToVr(message);
    }

    private void HandleBikeResistanceSettings(string settings)
    {
        Console.WriteLine($"Bike resistance settings: {settings}");

        var resistance = Encoding.UTF8.GetBytes(settings);
        // TODO: kijken of dit echt werkt
        bleHandler.SetResistance(resistance[0]);
    }

    private void HandleStartCommand()
    {
        vrHandler.StartSession();
    }

    private void HandleStopCommand()
    {
        vrHandler.StopSession();
    }

    private void HandleEmergencyStopCommand()
    {
        vrHandler.EmergencyStop();
    }

    private void HandleLoginConfirmation(string confirmation)
    {
        if (confirmation == "1")
        {
            loggedIn = true;
        }
        else
        {
            loggedIn = false;
        }
    }
}