using System.Globalization;
using System.Windows;

namespace Arts;

public partial class ClientWindow : Window, IDataUpdateCallback
{
    private string clientId;
    private NetworkProcessor networkProcessor;

    public ClientWindow(string clientId, NetworkProcessor networkProcessor)
    {
        InitializeComponent();
        this.networkProcessor = networkProcessor;
        this.networkProcessor.AddCallbackMember(this);
        this.clientId = clientId;
        TitleBlock.Text = clientId;
    }

    /**
     * <summary>
     * Methode die controleert of deze window de client is waarvan een update is gestuurd.
     * Bij true zal het GUI worden aangepast met de nieuwe waardes.
     * </summary>
     * <param name="clientId">De naam en geboortedatum van de client</param>>
     * <param name="data">De data van de client, zoals snelheid enz</param>
     */
    public void UpdateData(string clientId, string data)
    {
        if (this.clientId.Equals(clientId))
        {
            //todo handle data
            string[] dataSplit = data.Split(" ");
            //verwerk de data uit de array
            string speed = dataSplit[0];
            string distance = dataSplit[1];
            string power = dataSplit[2];
            string rpm = dataSplit[3];
            string heartRate = dataSplit[4];
            
            //werk de bijbehorende tekstblokken bij
            Dispatcher.Invoke(() =>
            {
                SpeedValueTextBlock.Text = speed;
                DistanceValueTextBlock.Text = distance;
                PowerValueTextBlock.Text = power;
                RpmValueTextBlock.Text = rpm;
                HeartRateValueTextBlock.Text = heartRate;
            });

        }
    }
    private void StartClientSessie(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Starting session!");
        networkProcessor.StartClientSessie(clientId);
        networkProcessor.AddActiveClient(clientId);

    }
    private void StopClientSessie(object sender, RoutedEventArgs e)
    {
        networkProcessor.StopClientSessie(clientId);
        Dispatcher.Invoke(() =>
            {
                SpeedValueTextBlock.Text = null;
                DistanceValueTextBlock.Text = null;
                PowerValueTextBlock.Text = null;
                RpmValueTextBlock.Text = null;
                HeartRateValueTextBlock.Text = null;
            });
    }
    private void EmergencyStopClientSessie(object sender, RoutedEventArgs e)
    {
        networkProcessor.EmergencyStopClientSessie(clientId);
    }
    private void SendPressed(object sender, RoutedEventArgs e)
    {
        if (ChatInputTextBox.Text.Length > 0)
        {
            networkProcessor.SendMessage(clientId, ChatInputTextBox.Text);
            ChatHistoryBox.Text += ChatInputTextBox.Text + "\n\n";
        }
    }

    private void ConfirmResistancePressed(object sender, RoutedEventArgs e)
    {
        //Pak de slider waarde en stuur het door naar de server
        //InvariantCulture zorgt voor een punt ("10.0") en niet een comma ("10,0")
        string resistanceValue = ResistanceSlider.Value.ToString("0.0", CultureInfo.InvariantCulture);
        networkProcessor.SendConfigs(clientId, resistanceValue);
    }

    private void GetHistoryClicked(object sender, RoutedEventArgs e)
    {
        
    }
}