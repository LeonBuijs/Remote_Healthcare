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

public partial class ClientWindow : Window, IDataUpdateCallback, INotifyPropertyChanged
{
    private string clientId;
    private int amoutOffGraphs = 4;
    private NetworkProcessor networkProcessor;    
    public SeriesCollection[] SeriesCollections { get; set; }
    public ObservableCollection<string>[] LabelsCollections { get; set; }
    public Func<double, string> Formatter { get; set; }

    public ClientWindow(string clientId, NetworkProcessor networkProcessor)
    {
        InitializeComponent();
        DataContext = this;
        this.networkProcessor = networkProcessor;
        this.networkProcessor.AddCallbackMember(this);
        this.clientId = clientId;
        TitleBlock.Text = clientId;
        //uncomment for test showcase
        //UpdateHistoryTextBlock("01-11-2024", "00:12:30", "3", "4", "5", "6", "7");
        InitializeGraphs();
    }

    /**
     * <summary>
     * In deze methode worden 4 verschillende grafieken aangemaakt,
     * waar later de gegevens van de geschiedenis van de client in komen te staan
     * </summary>
     *
     * <value> SeriesCollections is een lijst van 4 SeriesCollection 1 voor iedere grafiek. 
     * 1 seriesCollection is een verzamenling van alle data die in een grafiek moet komen met het type grafiek </value>
     * 
     * <value> LabelsCollections is een lijst van 4 ObservableCollections 1 voor iedere grafiek.
     * dit is een lijst die alle X_as waardes onthoud.</value>
     * 
     * <value> Formatter set de waardes voor de Y-as. Dit word generiek gedaan.
     * De "N" staat voor Number wat inhoud dat de y as allemaal nummers zijn en in dit Format met 2 cijfers achter de comma</value>
     */
    private void InitializeGraphs()
    {
        var charts = new []{Chart1, Chart2, Chart3, Chart4};
        // Initialiseer vier SeriesCollections en Labels
        SeriesCollections = new SeriesCollection[amoutOffGraphs];
        LabelsCollections = new ObservableCollection<string>[amoutOffGraphs];
        
        for (int i = 0; i < amoutOffGraphs; i++)
        {
            SeriesCollections[i] = new SeriesCollection
            {
                new LineSeries
                {
                    Title = $"Data Serie {i+1}",
                    Values = new ChartValues<double>{1,2,3,4,5}
                }
            };
            if (i<2)
            {
                SeriesCollections[i].Add(new LineSeries
                {
                    Title = $"Data Serie {i+1} lijn 2",
                    Values = new ChartValues<double>()
                });
            }
            LabelsCollections[i] = new ObservableCollection<string>{ "jan", "feb", "maart", "april", "mei" };
            charts[i].Series = SeriesCollections[i];
        }

        Formatter = value => value.ToString("N");
        
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
                        $"Total distance: {distance} meter";
        if (!string.IsNullOrEmpty(HistoryTextBlock.Text))
        {
            toShow = $"\n{toShow}";
        }
        HistoryTextBlock.Text += toShow;
    }

    /**
     * <summary>
     * Deze methode voegd nieuwe waardes toe aan de desbetreffende grafiek met de geschiedenis van data van de klant.
     * </summary>
     * <param name="chartIndex">Hiermee kies je in welke grafiek de data moet komen</param>
     * <param name="newValue">Dit is de nieuwe waarde die toegevoegd gaat worden aan de grafiek</param>
     * <param name="label">Dit is het bijpassende label wat op de X-as gezed gaat worden</param>
     */
    public void UpdateHistoryCharts(int chartIndex, double newValue, string label, int lineIndex = 0)
    {
        if (chartIndex < 0 || chartIndex >= SeriesCollections.Length) return;
        
        if (lineIndex < 0 || lineIndex >= SeriesCollections[chartIndex].Count) return;

        ((LineSeries)SeriesCollections[chartIndex][lineIndex]).Values.Add(newValue);
        LabelsCollections[chartIndex].Add(label);

        // eventueel om oudere lijnen te verwijderen als er teveel in de grafiek komen
        if (((LineSeries)SeriesCollections[chartIndex][lineIndex]).Values.Count > 20)
        {
            ((LineSeries)SeriesCollections[chartIndex][lineIndex]).Values.RemoveAt(0);
            LabelsCollections[chartIndex].RemoveAt(0);
        }

        Console.WriteLine($"Added {newValue} to chart {chartIndex} at line {lineIndex} with label {label}");

        OnPropertyChanged(nameof(SeriesCollections));
        OnPropertyChanged(nameof(LabelsCollections));
    }

    

    public string GetClientinfo()
    {
        return clientId;
    }

    /**
     * <summary>
     * Dit event wordt getriggerrd als een iegenschap van van deze klasse verranderd.
     * </summary>
     */
    public event PropertyChangedEventHandler? PropertyChanged;
    
    /**
     * <summary>
     * Deze methode roept het bovenstaande event aan. als dit event getriggerd is stuurdt hij een melding naar de GUI
     * dat er in deze window iets verrandert is. hij geeft dan mee welke property verranderd zodat de GUI hem kan aanpassen
     * </summary>
     * <param name="propertyName">Dit is de property name die verranderd gaat worden. hij is standaard null,
     * maar je initializeerd hem door  de methode aan te roepen met nameof(property) dan wordt de name van die property gebruikt</param>
     * 
     */
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        Console.WriteLine(propertyName + " changed");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        networkProcessor.GetDataHistory(clientId);
    }

    // protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    // {
    //     if (EqualityComparer<T>.Default.Equals(field, value)) return false;
    //     field = value;
    //     OnPropertyChanged(propertyName);
    //     return true;
    // }
}