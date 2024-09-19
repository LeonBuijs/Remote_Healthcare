using System.Net;
using System.Net.Sockets;

namespace Simulation;

public partial class Form1 : Form
{
    private TcpListener server;
    private NetworkStream networkStream;
    private TcpClient client;
    private Simulation simulation;
    
    public Form1()
    {
        InitializeComponent();
        this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        
        server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
        server.Start();
        client = server.AcceptTcpClient();
        networkStream = client.GetStream();
        
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        Label label1 = new Label()
        {
            Text = "&Speed",
            Location = new Point(10, 10),
            TabIndex = 0
        };

        TextBox field1 = new TextBox()
        {
            Location = new Point(label1.Location.X, label1.Bounds.Bottom + Padding.Top),
            TabIndex = 1
        };
        Label label2 = new Label()
        {
            Text = "&Wattage",
            Location = new Point(150, 10),
            TabIndex = 2
        };

        TextBox field2 = new TextBox()
        {
            Location = new Point(label2.Location.X, label2.Bounds.Bottom + Padding.Top),
            TabIndex = 3
        };
        
        Label label3 = new Label()
        {
            Text = "&RPM",
            Location = new Point(290, 10),
            TabIndex = 4
        };

        TextBox field3 = new TextBox()
        {
            Location = new Point(label3.Location.X, label3.Bounds.Bottom + Padding.Top),
            TabIndex = 5
        };
        
        Label label4 = new Label()
        {
            Text = "&HeartRate",
            Location = new Point(430, 10),
            TabIndex = 6
        };

        TextBox field4 = new TextBox()
        {
            Location = new Point(label4.Location.X, label4.Bounds.Bottom + Padding.Top),
            TabIndex = 7
        };
        Button sendButton = new Button()
        {
            Location = new Point(570, 10),
            TabIndex = 8,
            Text = "Send",
            Width = 100,
            Height = 50
        };

        sendButton.Click += (s, e) => SendData(field1, field2, field3, field4);
       
        Controls.Add(label1);
        Controls.Add(field1);
        
        Controls.Add(label2);
        Controls.Add(field2);
        
        Controls.Add(label3);
        Controls.Add(field3);
        
        Controls.Add(label4);
        Controls.Add(field4);
        Controls.Add(sendButton);
        
        
    }

    private void SendData(TextBox field1, TextBox field2, TextBox field3, TextBox field4)
    {
        Console.WriteLine($"Speed: {field1.Text}, Watt: {field2.Text}, RPM: {field3.Text}, HeartRate: {field4.Text}");
        simulation = new Simulation(Convert.ToInt32(field1.Text), Convert.ToInt32(field2.Text), Convert.ToInt32(field3.Text), Convert.ToInt32(field4.Text));
        Thread thread = new Thread(SendData);
        thread.Start();
    }

    private void SendData()
    {
        while (true)
        {
            byte[] array = simulation.GenerateData();
            Console.WriteLine(BitConverter.ToString(array).Replace("-", " "));
            networkStream.Write(array, 0, array.Length);
            Thread.Sleep(100);
        }
    }
    
    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        networkStream.Close();
        client.Close();
        server.Stop();
    }
}