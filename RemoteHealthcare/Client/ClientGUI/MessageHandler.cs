using System;
using System.Net.Sockets;
using System.Text;
using Avans.TI.BLE;

namespace ClientGUI
{
    public class MessageHandler
    {
        private ClientApplication _clientApp;
        private NetworkStream _stream;
        

        public MessageHandler(ClientApplication clientApp, NetworkStream stream)
        {
            _clientApp = clientApp;
            _stream = stream;
        }

        public async Task ReceiveMessages()
        {
            NetworkStream stream = _clientApp.Stream;

            while (true)
            {
                byte[] buffer = new byte[1024]; // Buffer om berichten te ontvangen
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    AnalyseMessage(message);
                }
            }
        }

        private void AnalyseMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            char identifier = message[0];
            string content = message.Substring(1);

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
                default:
                    Console.WriteLine("Unknown message type");
                    break;
            }
        }

        private void HandleChatMessage(string message)
        {
            Console.WriteLine($"Chat message from doctor: {message}");
            
        }

        private void HandleBikeResistanceSettings(string settings)
        {
            Console.WriteLine($"Bike resistance settings: {settings}");
            
            // todo add SetResistance methode hier
        }

        private void HandleStartCommand()
        {
            Console.WriteLine("Start command received");
            
        }

        private void HandleStopCommand()
        {
            Console.WriteLine("Stop command received");
            
        }

        private void HandleEmergencyStopCommand()
        {
            Console.WriteLine("Emergency stop command received");
            
        }

        private void HandleLoginConfirmation(string confirmation)
        {
            Console.WriteLine($"Login confirmation: {confirmation}");
            
        }
    }
}
