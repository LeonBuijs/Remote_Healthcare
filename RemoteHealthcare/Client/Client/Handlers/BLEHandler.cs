using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Avans.TI.BLE;
using Client;
using Form = Client.Form;

public class BLEHandler
    {
        private static BikeData bikeData = new();
        private static BLE bleBike = new();
        private static BLE bleHeart = new ();
        private static bool simulationMode;

        public BLEHandler()
        {
            TrySimulationMode();
        }

        /**
         * Methode om de BLE devices te starten na invoeren van deviceID
         * Bij ongeldig deviceID false returnen en apparaten niet verbinden
         */
        public bool Start(string deviceId)
        {
            if (simulationMode)
            {
                return false;
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

        private static async Task StartBLE()
        {
            int errorCode;
            // Connecting
            errorCode = await bleBike.OpenDevice("Tacx Flux 00472");
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
            // while (errorCode != 0)
            // {
            //     errorCode = await bleHeart.OpenDevice("Decathlon Dual HR");
            //     Console.WriteLine($"Heart: {errorCode}");
            //     Thread.Sleep(1000);
            // }

            errorCode = await bleHeart.SetService("HeartRate");
            Console.WriteLine($"HeartRate: {errorCode}");
            // while (errorCode != 0)
            // {
            //     errorCode = await bleHeart.SetService("HeartRate");
            //     Console.WriteLine($"Heart: {errorCode}");
            //     Thread.Sleep(1000);
            // }

            bleHeart.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            errorCode = await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");
            Console.WriteLine($"HeartRateMeasurement: {errorCode}");

            // while (errorCode != 0)
            // {
            //     errorCode = await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");
            //     Console.WriteLine($"Heart: {errorCode}");
            //     Thread.Sleep(1000);
            // }
        }

        /**
         * Methode om te controleren of het ingevoerde framenummer beschikbaar is
         */
        private static bool DeviceAvailable(string frameNumber)
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
        private static void BleBike_SubscriptionValueChanged(object Sender, BLESubscriptionValueChangedEventArgs e)
        {
            bikeData.UpdateData(BitConverter.ToString(e.Data).Replace("-", " "));
            Console.WriteLine(
                $"Speed: {bikeData.Speed} RPM: {bikeData.Rpm} Distance: {bikeData.Distance} Watts: {bikeData.Watt} Time: {bikeData.Time} HeartRate: {bikeData.HeartRate}");
        }

        /**
         * Hoofdmethode om data naar een fiets te sturen
         */
        private static async void SendMessageToBike(byte[] payload)
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
        private static void TrySimulationMode()
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
            
            while (true)
            {
                var stream = tcpClient.GetStream();

                var buffer1 = new byte[1024];
                var bytesRead1 = stream.Read(buffer1, 0, buffer1.Length);
                bikeData.UpdateData(BitConverter.ToString(buffer1, 0, bytesRead1).Replace("-", " "));
                Console.WriteLine(
                    $"Speed: {bikeData.Speed} RPM: {bikeData.Rpm} Distance: {bikeData.Distance} Watts: {bikeData.Watt} Time: {bikeData.Time} HeartRate: {bikeData.HeartRate}");
            }
        }
    }