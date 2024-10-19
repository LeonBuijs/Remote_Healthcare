namespace Client.Handlers;

public interface IBLECallback
{
    public void OnReceivedBikeData(BikeData bikeData);
}