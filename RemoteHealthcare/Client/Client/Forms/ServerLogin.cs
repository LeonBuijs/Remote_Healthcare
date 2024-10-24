using System;
using System.Threading;
using System.Windows.Forms;
using Client.Handlers;

namespace Client.Forms;

public partial class ServerLogin : Form
{
    private readonly MessageHandler messageHandler;

    private Connection connection;

    // TextBox attributen
    private string serverIp;
    private string firstName;
    private string lastName;
    private string birthDate;

    public ServerLogin(MessageHandler messageHandler)
    {
        this.messageHandler = messageHandler;
        InitializeComponent();
    }

    private void connectButton_Click(object sender, EventArgs e)
    {
        serverIp = serverIPTextBox.Text;
        firstName = firstNameTextBox.Text;
        lastName = lastNameTextBox.Text;
        birthDate = GetBirthDate();

        // Verification
        if (CheckTextBoxes())
        {
            MessageBox.Show("Enter all text areas", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var connected = ConnectToServer(serverIp);

            if (!connected)
            {
                MessageBox.Show("Invalid login details", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        catch (Exception)
        {
            MessageBox.Show("Unable to connect to server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Wanneer er verbinding is met de server, VR opstarten en verbinden
        ConnectToVr();
        
        Console.WriteLine("Connected!");
        Hide();
    }

    /**
     * Methode om verbinding te maken met de server
     * Returnt false als er geen toegang is, true zo wel
     */
    private bool ConnectToServer(string ip)
    {
        connection = new Connection(ip, 6666, messageHandler);

        var loginMessage = $"0 {firstName} {lastName} {birthDate}";
        connection.SendMessage(loginMessage);
        messageHandler.BleHandler.serverConnection = connection;
        
        Thread.Sleep(1000);
        
        return messageHandler.LoggedIn;
    }

    /**
     * Methode om de VR-connectie op te zetten
     */
    private void ConnectToVr()
    {
        // TEST CODE
        
        // var simProcess = new Process();
        // simProcess.StartInfo.FileName =
        //     @"C:\Users\jaspe\RiderProjects\Remote_Healthcare\RemoteHealthcare\Client\VRConnection\bin\Debug\net8.0\NetworkEngine.24.9.26\NetworkEngine\sim.bat";
        // simProcess.Start();
        
        // var processStartInfo = new ProcessStartInfo("cmd.exe", "/c " + @"C:\Users\jaspe\RiderProjects\Remote_Healthcare\RemoteHealthcare\Client\VRConnection\bin\Debug\net8.0\NetworkEngine.24.9.26\NetworkEngine\sim.bat");
        // simProcess = Process.Start(processStartInfo);
        //
        // Thread.Sleep(5000);
        
        // Process firstProc = new Process();
        // firstProc.StartInfo.FileName = "C:\\Users\\jaspe\\RiderProjects\\Remote_Healthcare\\RemoteHealthcare\\Client\\VRConnection\\bin\\Debug\\net8.0\\VRConnection.exe";
        // firstProc.ConnectDevices();
        
        // var vrProcess = new Process();
        // vrProcess.StartInfo.FileName = @"C:\Users\jaspe\RiderProjects\Remote_Healthcare\RemoteHealthcare\Client\VRConnection\bin\Debug\net8.0\VRConnection.exe";
        // vrProcess.Start();
        
        // TEST CODE

        var connected = false;

        while (!connected)
        {
            try
            {
                connection = new Connection("127.0.0.1", 9999, messageHandler);
                messageHandler.VrHandler = new VRHandler(connection);
                messageHandler.VrHandler.SendNameToVr(firstName, lastName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Thread.Sleep(1000);
                continue;
            }
            
            connected = true;
        }
    }

    /**
     * Methode om te controleren of alle velden ingevuld zijn
     */
    private bool CheckTextBoxes()
    {
        return string.IsNullOrWhiteSpace(serverIp) ||
               string.IsNullOrWhiteSpace(firstName) ||
               string.IsNullOrWhiteSpace(lastName) ||
               string.IsNullOrWhiteSpace(birthDate);
    }

    /**
     * Methode om een string met de geboortedatum te verkrijgen
     */
    private string GetBirthDate()
    {
        return dayTextBox.Text + monthTextBox.Text + yearTextBox.Text;
    }
}