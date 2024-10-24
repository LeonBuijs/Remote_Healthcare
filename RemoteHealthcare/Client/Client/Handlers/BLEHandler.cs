using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Avans.TI.BLE;

namespace Client.Handlers;

public class BLEHandler(MessageHandler messageHandler)
{
    private static bool running = true;
    private static bool simulationMode;

    private BikeData BikeData { get; } = new();
    private MessageHandler messageHandler = messageHandler;
    public Connection? serverConnection;

    private static BLE bleBike = new();
    private static BLE bleHeart = new();
    private static string BikeCharacteristic = "6e40fec2-b5a3-f393-e0a9-e50e24dcca9e";

    public int ErrorCodeBike { get; set; } = 1;
    public int ErrorCodeHeart { get; set; } = 1;

    /**
    * Methode om de BLE-devices te starten na invoeren van deviceID
    * Bij ongeldig deviceID false returnen en apparaten niet verbinden
    */
    public bool ConnectDevices(string bikeNumber)
    {
        TrySimulationMode();

        if (simulationMode)
        {
            ErrorCodeBike = 0;
            ErrorCodeHeart = 0;
            return true;
        }

        if (DeviceAvailable(bikeNumber))
        {
            Task.Run(async () =>
            {
                await StartBLE(bikeNumber);
            });
            return true;
        }
        
        return false;
    }

    /**
    * Methode om alle apparaten te starten en te verbinden
    */
    private async Task StartBLE(string bikeNumber)
    {
        await ConnectBike(bikeNumber);
        await ConnectHeart();
    }

    /**
    * Helper methode om met de fiets te verbinden
    */
    private async Task ConnectBike(string bikeNumber)
    {
        // Als fiets al verbonden is, returnen
        if (ErrorCodeBike == 0)
        {
            return;
        }

        // Connecting
        ErrorCodeBike = await bleBike.OpenDevice($"Tacx Flux {bikeNumber}");
        // Set service
        ErrorCodeBike = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
        // Subscribe
        bleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
        ErrorCodeBike = await bleBike.SubscribeToCharacteristic(BikeCharacteristic);

        Console.WriteLine($"Error code bike: {ErrorCodeBike}");
    }

    /**
    * Helper methode om met de hartslagmonitor te verbinden
    */
    private async Task ConnectHeart()
    {
        // Als hartslagmonitor al verbonden is, returnen
        if (ErrorCodeHeart == 0)
        {
            return;
        }

        // Connecting
        ErrorCodeHeart = await bleHeart.OpenDevice("Decathlon Dual HR");
        // Set service
        ErrorCodeHeart = await bleHeart.SetService("HeartRate");
        // Subscribe
        bleHeart.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
        ErrorCodeHeart = await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");

        Console.WriteLine($"Error code heart: {ErrorCodeHeart}");
    }

    /**
    * Methode om te controleren of het ingevoerde framenummer beschikbaar is
    */
    private bool DeviceAvailable(string frameNumber)
    {
        var deviceAvailable = false;

        var bleBikeList = bleBike.ListDevices();
        Console.WriteLine("Devices found: ");
        foreach (var name in bleBikeList)
        {
            Console.WriteLine($"Device: {name}");

            if ($"Tacx Flux {frameNumber}" == name)
            {
                deviceAvailable = true;
            }
        }

        return deviceAvailable;
    }


    /**
    * Aangeleverde klasse
    * Print waarde ontvangen uit fiets naar console
    */
    private void BleBike_SubscriptionValueChanged(object Sender, BLESubscriptionValueChangedEventArgs e)
    {
        BikeData.UpdateData(BitConverter.ToString(e.Data).Replace("-", " "));
        messageHandler.OnReceivedBikeData(BikeData);
        
        if (serverConnection != null)
        {
            serverConnection.SendMessage($"1 {BikeData}");
        }
    }

    /**
    * Hoofdmethode om data naar een fiets te sturen
    */
    private async void SendMessageToBike(byte[] payload)
    {
        byte sync = 0xA4;
        byte length = 0x09;
        byte msgId = 0x4E;
        byte channelNumber = 0x05;

        byte checksum = 0x00;
        checksum ^= sync;
        checksum ^= length;
        checksum ^= msgId;
        checksum ^= channelNumber;

        foreach (byte b in payload)
        {
            checksum ^= b;
        }

        var data = new byte[payload.Length + 5];
        data[0] = sync;
        data[1] = length;
        data[2] = msgId;
        data[3] = channelNumber;
        payload.CopyTo(data, 4);
        data[data.Length - 1] = checksum;

        int errorCode = await bleBike.WriteCharacteristic(
            "6e40fec3-b5a3-f393-e0a9-e50e24dcca9e"
            , data);
        Console.WriteLine($"sending message with code: {errorCode}");
    }

    /**
    * Specifieke methode om de weerstand van de fiets aan te passen
    */
    public void SetResistance(int resistance)
    {
        if (simulationMode)
        {
            return;
        }

        byte resistancePage = 0x30;
        byte zero = 0x00;
        byte[] payload = [resistancePage, zero, zero, zero, zero, zero, zero, (byte)resistance];
        SendMessageToBike(payload);
    }

    /**
    * Methode om verbinding te maken met de simulator applicatie
    * Als de sim niet live is, returnen
    */
    private void TrySimulationMode()
    {
        TcpClient tcpClient;
        try
        {
            tcpClient = new TcpClient("127.0.0.1", 8080);
        }
        catch (Exception)
        {
            return;
        }

        simulationMode = true;

        var simThread = new Thread(() =>
        {
            while (running)
            {
                var stream = tcpClient.GetStream();
                var buffer = new byte[32];
                var bytesRead = stream.Read(buffer, 0, buffer.Length);

                BikeData.UpdateData(BitConverter.ToString(buffer, 0, bytesRead).Replace("-", " "));
                messageHandler.OnReceivedBikeData(BikeData);
                
                if (serverConnection != null)
                {
                    serverConnection.SendMessage($"1 {BikeData}");
                }
            }
        });
        simThread.Start();
    }

    /**
    * Methode om de BLE-verbindingen te onderbreken
    */
    public void Disconnect()
    {
        running = false;

        bleBike.Unsubscribe(BikeCharacteristic);
        bleBike.CloseDevice();
        bleHeart.Unsubscribe("HeartRateMeasurement");
        bleHeart.CloseDevice();
    }
}