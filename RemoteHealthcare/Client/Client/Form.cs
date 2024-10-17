using System;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Client;

public partial class Form : System.Windows.Forms.Form
{
    private BLEHandler bleHandler;
    private ClientApplication _clientApp;
    private TcpClient _tcpClient;
        
    public Form(BLEHandler bleHandler)
    {
        this.bleHandler = bleHandler;
        InitializeComponent();
    }
        
    private void connectButton_Click(object sender, EventArgs e)
    {
        var serverIp = "127.0.0.1";
        var deviceId = "00472";
        var firstName = firstNameTextBox.Text;
        var lastName = lastNameTextBox.Text;
        var birthDate = birthDateTextBox.Text;
            
        // Verification
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(birthDate))
        {
            MessageBox.Show("Vul alle velden in !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
            
        try
        {
            ConnectToServer(serverIp, deviceId, firstName, lastName, birthDate);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fout bij het verbinden {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        //Start sending data...
        // await _clientApp.Start(); todo
        Hide();
    }
    private void ConnectToServer(string ip, string deviceId, string firstName, string lastName, string birthDate)
    {
        bleHandler.Start(deviceId);
            
        _clientApp = new ClientApplication(ip, 6666, null);//todo
            
        var loginMessage = GetIndex(firstName, lastName, birthDate);
        _clientApp.SendMessage(loginMessage); // Send login to Server
            
        // await _clientApp.Start();
        Console.WriteLine("Connected to server");
    }
                                    
    private static string GetIndex(string firstName, string lastName, string birthDate)
    {
        return $"0 {firstName} {lastName} {birthDate}";
    }
}