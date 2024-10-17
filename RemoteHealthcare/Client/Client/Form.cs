using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClientGUI;

namespace Client
{
    public partial class Form : System.Windows.Forms.Form
    {
        private ClientApplication _clientApp;
        private TcpClient _tcpClient;
        
        public Form()
        {
            InitializeComponent();
        }
        
        private async void connectButton_Click(object sender, EventArgs e)
        {
            string firstName = firstNameTextBox.Text;
            string lastName = lastNameTextBox.Text;
            string birthDate = birthDateTextBox.Text;
            
            // Verification
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(birthDate))
            {
                MessageBox.Show("Vul alle velden in !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                await ConnectToServer(firstName, lastName, birthDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het verbinden {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Start sending data...
            await _clientApp.Start();
            Hide();
        }
        private async Task ConnectToServer(string firstName, string lastName, string birthDate)
        {
            string ipAddress = "127.0.0.1";
            int port = 6666;
            _clientApp = new ClientApplication(ipAddress, port, null);//todo
            
            string identificationMessage = FormatIdentificationMessage(firstName, lastName, birthDate);
            await _clientApp.SendMessage(identificationMessage); // Send identification to Server
            
            // Storage data
            Storage storage = new Storage();
            storage.AddData(identificationMessage);
            
            // await _clientApp.Start();
            Console.WriteLine("Connected to server");
        }
                                    
        private string FormatIdentificationMessage(string firstName, string lastName, string birthDate)
        {
            return $"0 {firstName} {lastName} {birthDate}";
        }
    }
}