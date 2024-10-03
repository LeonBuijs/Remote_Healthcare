using Avans.TI.BLE;

namespace ClientGUI;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    
    private static BikeData bikeData = new BikeData();
    
    static async Task Main()
    {
        int errorCode;
        BLE bleBike = new BLE();
        BLE bleHeart = new BLE();
        Thread.Sleep(1000);
        
        // List available devices
            List<String> bleBikeList = bleBike.ListDevices();   
            Console.WriteLine("Devices found: ");
            foreach (var name in bleBikeList)
            {
                Console.WriteLine($"Device: {name}");
            }

            // Connecting
            errorCode = await bleBike.OpenDevice("Tacx Flux 01140");
            Console.WriteLine($"BikeOpen: {errorCode}");

            while (errorCode != 0)
            {
                errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
                Thread.Sleep(1000);
                Console.WriteLine($"BikeOpen: {errorCode}");
            }


            var services = bleBike.GetServices;
            foreach (var service in services)
            {
                Console.WriteLine($"Service: {service}");
            }

            // Set service
            errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
            while (errorCode != 0)
            {
                errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
            }

            Console.WriteLine($"Bike: {errorCode}");

            // Subscribe
            bleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            errorCode = await bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");
            Console.WriteLine($"BikeSubscription: {errorCode}");

            // Heart rate
            errorCode = await bleHeart.OpenDevice("Decathlon Dual HR");
            Console.WriteLine($"Heart: {errorCode}");
            while (errorCode != 0)
            {
                errorCode = await bleHeart.OpenDevice("Decathlon Dual HR");
                Console.WriteLine($"Heart: {errorCode}");
                Thread.Sleep(1000);
            }

            errorCode = await bleHeart.SetService("HeartRate");
            Console.WriteLine($"HeartRate: {errorCode}");
            while (errorCode != 0)
            {
                errorCode = await bleHeart.SetService("HeartRate");
                Console.WriteLine($"Heart: {errorCode}");
                Thread.Sleep(1000);
            }

            bleHeart.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            errorCode = await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");
            Console.WriteLine($"HeartRateMeasurement: {errorCode}");

            while (errorCode != 0)
            {
                errorCode = await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");
                Console.WriteLine($"Heart: {errorCode}");
                Thread.Sleep(1000);
            }
        
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
    
    private static void BleBike_SubscriptionValueChanged(object Sender, BLESubscriptionValueChangedEventArgs e)
    {
        bikeData.UpdateData(BitConverter.ToString(e.Data).Replace("-", " "));
        Console.WriteLine(
            $"Speed: {bikeData.speed} RPM: {bikeData.rpm} Distance: {bikeData.distance} Watts: {bikeData.watt} Time: {bikeData.time} HeartRate: {bikeData.heartRate}");
    }
        
    /**
     * Hoofdmethode om data naar een fiets te sturen
     */
    private static async void SendMessageToBike(byte[] payload, BLE bleBike)
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
            
        byte[] data = new byte[payload.Length + 5];
        data[0] = sync;
        data[1] = length;
        data[2] = msgId;
        data[3] = channelNumber;
        payload.CopyTo(data, 4);
        data[data.Length-1] = checksum;
            
        int errorCode = await bleBike.WriteCharacteristic(
            "6e40fec3-b5a3-f393-e0a9-e50e24dcca9e"
            , data);
        Console.WriteLine($"sending message with code: {errorCode}" );
    }

    /**
     * Specefieke methode om de weerstand van de fiets aan te passen
     */
    public static void setResistance(byte resistance, BLE bike)
    {
        byte resistancePage = 0x30;
        byte zero = 0x00;
        byte[] payload = { resistancePage, zero, zero, zero, zero, zero, zero, resistance };
        SendMessageToBike(payload, bike);
    }
}