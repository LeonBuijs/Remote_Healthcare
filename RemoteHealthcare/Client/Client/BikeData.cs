namespace Client;

public class BikeData
{
    private bool initialized;

    public int Speed { get; set; }
    public int Watt { get; set; }
    public int Rpm { get; set; }
    public int HeartRate { get; set; }

    //attributen voor tijd
    public double Time { get; set; } //todo mogelijk implementeren nog
    private double lastTime;
    private int timeFlip;
    private double timeOffset;

    //attributen voor afstanden
    public int Distance { get; set; }
    private int lastDistance;
    private int distanceFlip;
    private int distanceOffset;

    public void UpdateData(string data)
    {
        var split = data.Split(' ');
        var length = split.Length;
        switch (length)
        {
            case 4:
                // heartRate = data[2];
                HeartRate = int.Parse(split[1], System.Globalization.NumberStyles.HexNumber);
                break;
            case 6:
                //todo andere data van hartslagmeter, mogelijk nog extra data implementeren.
                break;
            case 13:
                ChangeBikeData(split);
                break;
        }
    }

    private void ChangeBikeData(string[] split)
    {
        var pageNumber = int.Parse(split[4], System.Globalization.NumberStyles.HexNumber);

        switch (pageNumber)
        {
            case 16:
                Speed = int.Parse(split[9], System.Globalization.NumberStyles.HexNumber);

                CalculateTime(split);

                CalculateDistance(split);

                if (!initialized)
                {
                    Time = 0;
                    distanceOffset = Distance;
                    initialized = true;
                }

                break;
            case 25:
                Rpm = int.Parse(split[6], System.Globalization.NumberStyles.HexNumber);
                Watt = int.Parse(split[9], System.Globalization.NumberStyles.HexNumber);
                break;
        }
    }

    private void CalculateDistance(string[] split)
    {
        //distance flipt na 255
        Distance = int.Parse(split[7], System.Globalization.NumberStyles.HexNumber);
        if (lastDistance > 200 && Distance < 100)
        {
            distanceFlip++;
        }

        lastDistance = Distance;

        Distance += distanceFlip * 256 - distanceOffset;
    }

    private void CalculateTime(string[] split)
    {
        //time flipt na 255
        var tempTime = int.Parse(split[6], System.Globalization.NumberStyles.HexNumber);

        var difference = tempTime - lastTime;

        if (difference < 0)
        {
            Time += 256 + difference;
        }
        else
        {
            Time += difference * 0.25;
        }

        if (lastTime > 200 && tempTime < 100)
        {
            timeFlip++;
        }

        lastTime = tempTime;
    }
}