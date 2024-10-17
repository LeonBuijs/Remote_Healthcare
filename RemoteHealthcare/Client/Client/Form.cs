using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using ClientGUI;

namespace Client;

public partial class Form : System.Windows.Forms.Form
{
    private BLEHandler bleHandler = new();
    private VRHandler vrHandler = new();
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
        messageHandler = new MessageHandler(bleHandler, vrHandler);
        InitializeComponent();
    }

    private void connectButton_Click(object sender, EventArgs e)
    {
        serverIp = serverIPTextBox.Text;
        deviceId = bikeNumberTextBox.Text;
        firstName = firstNameTextBox.Text;
        lastName = lastNameTextBox.Text;
        birthDate = birthDateTextBox.Text;

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
}