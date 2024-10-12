using System.Net;
using System.Net.Sockets;

namespace Simulation
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private NetworkStream networkStream;
        private TcpClient client;
        private Simulation simulation;
        private Thread sendThread;

        public Form1()
        {
            InitializeComponent(); 
            this.FormClosing += Form1_FormClosing;
            
            Thread serverThread = new Thread(StartServer);
            serverThread.IsBackground = true;
            serverThread.Start();
            
            sendButton.Click += (sender, args) => SendData();
        }

        private void StartServer()
        {
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
            server.Start();
            client = server.AcceptTcpClient();
            networkStream = client.GetStream();
        }

        private void SendData()
        {
            
            if (int.TryParse(field1.Text, out int speed) &&
                int.TryParse(field2.Text, out int wattage) &&
                int.TryParse(field3.Text, out int rpm) &&
                int.TryParse(field4.Text, out int heartRate))
            {
                simulation = new Simulation(speed, wattage, rpm, heartRate);
                
                if (sendThread == null || !sendThread.IsAlive)
                {
                    sendThread = new Thread(SendDataThread);
                    sendThread.IsBackground = true;
                    sendThread.Start();
                }
            }
            else
            {
                MessageBox.Show("Voer geldige waarden in !");
            }
        }

        private void SendDataThread()
        {
            try
            {
                while (true)
                {
                    byte[] array = simulation.GenerateData();
                    networkStream.Write(array, 0, array.Length);
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het verzenden van data ! : {ex.Message}");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (networkStream != null)
                networkStream.Close();
            if (client != null)
                client.Close();
            if (server != null)
                server.Stop();
        }
    }
}