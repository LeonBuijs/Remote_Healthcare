using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;

namespace Arts;

public partial class ClientWindow : Window, IDataUpdateCallback
{
    private string clientId;
    private int amoutOffGraphs = 4;
    private NetworkProcessor networkProcessor;    
    private ChartViewModel chartViewModel;
    
    public ClientWindow(string clientId, NetworkProcessor networkProcessor)
    {
        InitializeComponent();
        chartViewModel = new ChartViewModel();
        DataContext = chartViewModel;
        this.networkProcessor = networkProcessor;
        this.networkProcessor.AddCallbackMember(this);
        this.clientId = clientId;
        TitleBlock.Text = clientId;
        //uncomment for test showcase
        //UpdateHistoryTextBlock("01-11-2024", "00:12:30", "3", "4", "5", "6", "7");
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
            string[] dataSplit = data.Split(" ");
            //verwerk de data uit de array
            string speed = dataSplit[0];
            string distance = dataSplit[1];
            string power = dataSplit[2];
            string time = dataSplit[3];
            string rpm = dataSplit[4];
            string heartRate = dataSplit[5];
            
            //werk de bijbehorende tekstblokken bij
            Dispatcher.Invoke(() =>
            {
                SpeedValueTextBlock.Text = speed;
                DistanceValueTextBlock.Text = distance;
                TimeValueTextBlock.Text = time;
                PowerValueTextBlock.Text = power;
                RpmValueTextBlock.Text = rpm;
                HeartRateValueTextBlock.Text = heartRate;
            });
        }
    }

    public void UpdateHistoryTextBlock(string date, string duration, string averageSpeed, string maxSpeed,
        string averageHeartRate, string maxHeartRate, string distance)
    {
        string toShow = $"{date}, {duration}\n" +
                        $"{averageSpeed} km/h avg, {maxSpeed} km/h max\n" +
                        $"{averageHeartRate} bpm avg, {maxHeartRate} bpm max\n" +
                        $"Total distance: {distance} meter" +
                        $"\n------------------------";
        
        //work on UI thread when using UI features
        Dispatcher.Invoke(() =>
        {
            if (!string.IsNullOrEmpty(HistoryTextBlock.Text))
            {
                toShow = $"\n{toShow}";
            }

            HistoryTextBlock.Text += toShow;
        });
    }

    /**
     * <summary>
     * Deze methode voegd nieuwe waardes toe aan de desbetreffende grafiek met de geschiedenis van data van de klant.
     * </summary>
     * <param name="chartIndex">Hiermee kies je in welke grafiek de data moet komen</param>
     * <param name="newValue">Dit is de nieuwe waarde die toegevoegd gaat worden aan de grafiek</param>
     * <param name="label">Dit is het bijpassende label wat op de X-as gezed gaat worden</param>
     */
    public void UpdateCharts(int chartIndex, double newValue, string label, int lineIndex = 0)
    {
        Dispatcher.Invoke(() =>
        {
            chartViewModel.UpdateCharts(chartIndex, newValue, label, lineIndex);
        });
    }
    
    public string GetClientinfo()
    {
        return clientId;
    }
    
    private void StartClientSession(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Starting session!");
        networkProcessor.StartClientSession(clientId);
        networkProcessor.AddActiveClient(clientId);

    }

    private void StopClientSession(object sender, RoutedEventArgs e)
    {
        networkProcessor.StopClientSession(clientId);
        Dispatcher.Invoke(() =>
            {
                SpeedValueTextBlock.Text = null;
                DistanceValueTextBlock.Text = null;
                TimeValueTextBlock.Text = null;
                PowerValueTextBlock.Text = null;
                RpmValueTextBlock.Text = null;
                HeartRateValueTextBlock.Text = null;
            });
    }

    private void EmergencyStopClientSession(object sender, RoutedEventArgs e)
    {
        networkProcessor.EmergencyStopClientSession(clientId);
    }

    private void SendPressed(object sender, RoutedEventArgs e)
    {
        if (ChatInputTextBox.Text.Length > 0)
        {
            networkProcessor.SendMessage(clientId, ChatInputTextBox.Text);
            ChatHistoryBox.Text += ChatInputTextBox.Text + "\n";
            ChatInputTextBox.Clear();
        }
    }

    private void ConfirmResistancePressed(object sender, RoutedEventArgs e)
    {
        //Pak de slider waarde en stuur het door naar de server
        //InvariantCulture zorgt voor een punt ("10.0") en niet een comma ("10,0")
        string resistanceValue = ResistanceSlider.Value.ToString("0", CultureInfo.InvariantCulture);
        networkProcessor.SendConfigs(clientId, resistanceValue);
    }

    private void GetHistoryClicked(object sender, RoutedEventArgs e)
    {
        //Voordat je de geschiedenis toont. De oude geschiedenis verwijderen.
        ResetHistoryData();
        networkProcessor.GetDataHistory(clientId);
    }
    public void ResetHistoryData()
    {
        HistoryTextBlock.Text = string.Empty;
        chartViewModel.ResetCharts();
    }
}