using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Client.Handlers;

namespace Client.Forms;

public partial class DeviceConnection : Form
{
    private MessageHandler messageHandler;

    // private BLEHandler bleHandler;
    public DeviceConnection()
    {
        InitializeComponent();
        messageHandler = new MessageHandler();
        // bleHandler = new BLEHandler(ref messageHandler);

        FormClosing += CloseForm;
    }

    private void connectButton_Click(object sender, EventArgs e)
    {
        var bikeNumber = CheckBikeNumberBox();

        if (bikeNumber == "" || !bikeNumber.All(char.IsDigit))
        {
            CreateWarningPopup("Invalid Bike Number", "Please enter a valid bike number.");
            return;
        }

        if (!messageHandler.BleHandler.ConnectDevices(bikeNumber))
        {
            CreateWarningPopup("Selected Bike Not Available",
                "Make sure that the bike is turned on and that the bike number is correct.");
            return;
        }

        // Check voor sim mode, wanneer sim mode aan is, verbind deel overslaan
        if (messageHandler.BleHandler.ErrorCodeBike == 0 && messageHandler.BleHandler.ErrorCodeHeart == 0)
        {
            StartServerLogin();
            return;
        }

        // Timeout om de bleHandler tijd te geven om te verbinden
        Thread.Sleep(5000);

        var bikeConnected = messageHandler.BleHandler.ErrorCodeBike;
        var heartRateConnected = messageHandler.BleHandler.ErrorCodeHeart;

        if (bikeConnected == 0 && heartRateConnected == 0)
        {
            StartServerLogin();
            return;
        }

        UpdateConnectionStatus();
    }

    /**
     * Methode om de huidige form te sluiten en de ServerLogin form te starten
     */
    private void StartServerLogin()
    {
        Console.WriteLine("ServerLogin Started!");

        Hide();
        var serverLogin = new ServerLogin(messageHandler);
        serverLogin.Closed += (s, args) => Close();
        serverLogin.Show();
    }

    /**
     * Methode voor het updaten van de connection status labels
     */
    private void UpdateConnectionStatus()
    {
        var errorcodeBike = messageHandler.BleHandler.ErrorCodeBike;
        var errorcodeHeart = messageHandler.BleHandler.ErrorCodeHeart;
        if (errorcodeBike == 0)
        {
            BikeConnectedStatusLabel.Text = "Connected!";
        }
        else
        {
            BikeConnectedStatusLabel.Text = "NOT CONNECTED";
        }

        if (errorcodeHeart == 0)
        {
            HeartRateConnectedStatusLabel.Text = "Connected!";
        }
        else
        {
            HeartRateConnectedStatusLabel.Text = "NOT CONNECTED";
        }

        if (errorcodeBike > 0 && errorcodeHeart > 0)
        {
            CreateWarningPopup("Connecting Failed", "Please try connecting again");
        }
        else if (errorcodeBike > 0 && errorcodeHeart == 0)
        {
            CreateWarningPopup("Connecting With Bike Failed", "Please try connecting again");
        }
        else if (errorcodeBike == 0 && errorcodeHeart > 0)
        {
            CreateWarningPopup("Connecting With Heart Rate Monitor Failed", "Please try connecting again");
        }
    }

    /**
     * Methode om een waarschuwing-popup te maken
     */
    private static void CreateWarningPopup(string title, string message)
    {
        MessageBox.Show($"{title}\n{message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /**
     * Methode voor het controleren van een geldige input van de gebruiker
     */
    private string CheckBikeNumberBox()
    {
        var bikeNumber = bikeNumberTextBox.Text;
        var correctLength = bikeNumber.Length == 5;

        if (string.IsNullOrWhiteSpace(bikeNumber) || !correctLength)
        {
            return "";
        }

        return bikeNumber;
    }

    /**
     * Methode om alle lopende verbindingen etc. netjes af te sluiten
     */
    private void CloseForm(object sender, FormClosingEventArgs e)
    {
        messageHandler.Disconnect();
    }
}