namespace Arts;

/**
 * Callback voor als er data binnenkomt.
 * Wanneer een nieuw venster opent, zal het zich aan de callback lijst toevoegen.
 * Zodat het hierna kan worden aangeroepen met binnenkomende data.
 */
public interface IDataUpdateCallback
{
    void UpdateData(string clientId, string data);
    void UpdateHistoryTextBlock(string date, string duration, string averageSpeed, string maxSpeed, string averageHeartRate, string maxHeartRate, string distance);
    void UpdateCharts(int chartIndex, double newValue, string label, int lineIndex = 0);
    void ResetHistoryData();
    string GetClientinfo();
}