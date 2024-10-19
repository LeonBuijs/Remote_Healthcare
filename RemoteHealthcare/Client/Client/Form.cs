using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Client.Handlers;
using ClientGUI;

namespace Client;

public partial class Form : System.Windows.Forms.Form, IBLECallback
{
    private BLEHandler bleHandler;
    private MessageHandler messageHandler;

    private Connection connection;

    // TextBox attributen
    private string serverIp;
    private string deviceId;
    private string firstName;
    private string lastName;
    private string birthDate;

    public Form()
    {
        bleHandler = new BLEHandler(this);
        messageHandler = new MessageHandler(bleHandler);
        InitializeComponent();
    }

    private void connectButton_Click(object sender, EventArgs e)
    {
        serverIp = serverIPTextBox.Text;
        deviceId = bikeNumberTextBox.Text;
        firstName = firstNameTextBox.Text;
        lastName = lastNameTextBox.Text;
        birthDate = GetBirthDate();

        // Verification
        if (CheckTextBoxes())
        {
            MessageBox.Show("Vul alle velden in !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var connected = ConnectToServer(serverIp, deviceId, firstName, lastName, birthDate);

            if (!connected)
            {
                MessageBox.Show("Invalid login details", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fout bij het verbinden {ex.Message}", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }
        //Start sending data... todo

        Console.WriteLine("Connected!");
        Hide();
    }

    private bool ConnectToServer(string ip, string deviceId, string firstName, string lastName, string birthDate)
    {
        // bleHandler.Start(deviceId); todo
        
        // Process firstProc = new Process();
        // firstProc.StartInfo.FileName = "C:\\Users\\jaspe\\RiderProjects\\Remote_Healthcare\\RemoteHealthcare\\Client\\VRConnection\\bin\\Debug\\net8.0\\VRConnection.exe";
        //
        // firstProc.Start();

        connection = new Connection(ip, 6666, messageHandler);

        var loginMessage = $"0 {firstName} {lastName} {birthDate}";
        connection.SendMessage(loginMessage);

        Thread.Sleep(2500);

        return messageHandler.loggedIn;
    }

    private bool CheckTextBoxes()
    {
        return string.IsNullOrWhiteSpace(serverIp) ||
               string.IsNullOrWhiteSpace(deviceId) ||
               string.IsNullOrWhiteSpace(firstName) ||
               string.IsNullOrWhiteSpace(lastName) ||
               string.IsNullOrWhiteSpace(birthDate);
    }

    private string GetBirthDate()
    {
        return dayTextBox.Text + monthTextBox.Text + yearTextBox.Text;
    }

    public void OnReceivedBikeData(string message)
    {
        // todo
    }
}