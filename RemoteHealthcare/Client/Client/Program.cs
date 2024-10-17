using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Avans.TI.BLE;

namespace Client
{
    class Program
    {
        private static BLEHandler ble;
        static async Task Main(string[] args)
        {
            // int errorCode;
            // bleBike = new BLE();
            // bleHeart = new BLE();
            Thread.Sleep(1000); // We need some time to list available devices

            ble = new BLEHandler();
            // ConnectToServer();

            // StartSimulation(); todo uitcommenten voor simulator

            // List available devices
            // List<String> bleBikeList = bleBike.ListDevices();
            // Console.WriteLine("Devices found: ");
            // foreach (var name in bleBikeList)
            // {
            //     Console.WriteLine($"Device: {name}");
            // }

            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form(ble));

            Console.Read();
        }
    }
    
}