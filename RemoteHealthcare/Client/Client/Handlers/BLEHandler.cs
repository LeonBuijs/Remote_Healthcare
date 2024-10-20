using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Avans.TI.BLE;
using Client;
using ClientGUI;

public class BLEHandler
{
    private MessageHandler messageHandler;

    private string bikeNumber;
    public BikeData bikeData { get; } = new();
    private static BLE bleBike = new();
    private static BLE bleHeart = new();
    private static bool simulationMode;

    public int ErrorCodeBike { get; set; } = 1;
    public int ErrorCodeHeart { get; set; } = 1;

    public BLEHandler(MessageHandler messageHandler, string bikeNumber)
    {
        this.messageHandler = messageHandler;
        this.bikeNumber = bikeNumber;
        TrySimulationMode();
    }

    /**
     * Methode om de BLE devices te starten na invoeren van deviceID
     * Bij ongeldig deviceID false returnen en apparaten niet verbinden
     */
    public bool ConnectDevices(string deviceId)
    {
        if (simulationMode)
        {
            ErrorCodeBike = 0;
            ErrorCodeHeart = 0;
            return true;
        }

        if (DeviceAvailable(deviceId))
        {
            Task.Run(async () =>
            {
                await StartBLE();
                return true;
            });
        }

        return false;
    }

    /**
     * Methode om alle apparaten te starten
     */
    private async Task StartBLE()
    {
        await ConnectBike();
        await ConnectHeart();
    }

    /**
     * Helper methode om met de fiets te verbinden
     */
    private async Task ConnectBike()
    {
        int errorCode;
        // Connecting
        errorCode = await bleBike.OpenDevice("Tacx Flux 00472");
        Console.WriteLine($"BikeOpen: {errorCode}");

        // Set service
        errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");


        Console.WriteLine($"Bike: {errorCode}");

        // Subscribe
        bleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
        errorCode = await bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");
        Console.WriteLine($"BikeSubscription: {errorCode}");
    }

    /**
     * Helper methode om met de hartslagmonitor te verbinden
     */
    private async Task ConnectHeart()
    {
        int errorCode;


        // Heart rate
        errorCode = await bleHeart.OpenDevice("Decathlon Dual HR");
        Console.WriteLine($"Heart: {errorCode}");


        errorCode = await bleHeart.SetService("HeartRate");
        Console.WriteLine($"HeartRate: {errorCode}");


        bleHeart.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
        errorCode = await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");
        Console.WriteLine($"HeartRateMeasurement: {errorCode}");
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
        bikeData.UpdateData(BitConverter.ToString(e.Data).Replace("-", " "));
        messageHandler.OnReceivedBikeData(bikeData);
        Console.WriteLine(
            $"Speed: {bikeData.Speed} RPM: {bikeData.Rpm} Distance: {bikeData.Distance} Watts: {bikeData.Watt} Time: {bikeData.Time} HeartRate: {bikeData.HeartRate}");
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
    public void SetResistance(byte resistance)
    {
        if (simulationMode)
        {
            return;
        }

        byte resistancePage = 0x30;
        byte zero = 0x00;
        byte[] payload = [resistancePage, zero, zero, zero, zero, zero, zero, resistance];
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
            while (true)
            {
                var stream = tcpClient.GetStream();
                var buffer = new byte[1024];
                var bytesRead = stream.Read(buffer, 0, buffer.Length);

                bikeData.UpdateData(BitConverter.ToString(buffer, 0, bytesRead).Replace("-", " "));
                messageHandler.OnReceivedBikeData(bikeData);
                Console.WriteLine(
                    $"Speed: {bikeData.Speed} RPM: {bikeData.Rpm} Distance: {bikeData.Distance} Watts: {bikeData.Watt} Time: {bikeData.Time} HeartRate: {bikeData.HeartRate}");
            }
        });
        simThread.Start();
    }
}