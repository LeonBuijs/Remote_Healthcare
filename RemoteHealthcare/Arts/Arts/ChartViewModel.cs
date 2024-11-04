using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveCharts;
using LiveCharts.Wpf;

namespace Arts;

public class ChartViewModel : INotifyPropertyChanged
{
    public SeriesCollection[] SeriesCollections { get; set; }
    public ObservableCollection<string>[] LabelsCollections { get; set; }
    public Func<double, string> Formatter { get; set; }
    private int amoutOffGraphs = 4;
    private string[] firstLineLabels = ["Duration", "Average speed", "Average heart rate", "Distance"];
    private string[] secondLineLabels = ["FillerValue","Maximum speed", "Maximum heart rate"];
    
    public ChartViewModel()
    {
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
        // Initialiseer vier SeriesCollections en Labels
        SeriesCollections = new SeriesCollection[amoutOffGraphs];
        LabelsCollections = new ObservableCollection<string>[amoutOffGraphs];
        
        for (int i = 0; i < amoutOffGraphs; i++)
        {
            SeriesCollections[i] = new SeriesCollection
            {
                new LineSeries
                {
                    Title = firstLineLabels[i],
                    Values = new ChartValues<double>{1,2,3,4,5}
                }
            };
            if (i<3)
            {
                SeriesCollections[i].Add(
                    new LineSeries
                {
                    Title = secondLineLabels[i],
                    Values = new ChartValues<double>()
                });
            }
            LabelsCollections[i] = new ObservableCollection<string>{ "jan", "feb", "maart", "april", "mei" };
        }

        Formatter = value => value.ToString("N");
        
    }
    
    public void UpdateHistoryCharts(int chartIndex, double newValue, string label, int lineIndex = 0)
    {
        if (chartIndex < 0 || chartIndex >= SeriesCollections.Length) return;

        if (lineIndex < 0 || lineIndex >= SeriesCollections[chartIndex].Count) return;

        ((LineSeries)SeriesCollections[chartIndex][lineIndex]).Values.Add(newValue);
        LabelsCollections[chartIndex].Add(label);

        // Optional: remove older lines if too many in the chart
        if (((LineSeries)SeriesCollections[chartIndex][lineIndex]).Values.Count > 20)
        {
            ((LineSeries)SeriesCollections[chartIndex][lineIndex]).Values.RemoveAt(0);
            LabelsCollections[chartIndex].RemoveAt(0);
        }

        Console.WriteLine($"Added {newValue} to chart {chartIndex} at line {lineIndex} with label {label}");

        OnPropertyChanged(nameof(SeriesCollections));
        OnPropertyChanged(nameof(LabelsCollections));
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
}