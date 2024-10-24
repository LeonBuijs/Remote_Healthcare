namespace Client;

public class BikeData
{
    private bool initialized;

    private int Speed;
    private int Watt;
    private int Rpm;

    private int HeartRate;

    //attributen voor tijd
    private double Time;
    private double lastTime;
    private int timeFlip;
    private double timeOffset;

    //attributen voor afstanden
    private int Distance { get; set; }
    private int lastDistance;
    private int distanceFlip;
    private int distanceOffset;

    /**
     * Methode om het bikeData object te updaten
     */
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
            case 13:
                ChangeBikeData(split);
                break;
        }
    }

    /**
     * Methode om data van de fiets aan te passen
     */
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

    /**
     * Helper methode om de afstand te berekenen
     * Databyte van afstand flipt na 255
     */
    private void CalculateDistance(string[] split)
    {
        Distance = int.Parse(split[7], System.Globalization.NumberStyles.HexNumber);
        if (lastDistance > 200 && Distance < 100)
        {
            distanceFlip++;
        }

        lastDistance = Distance;

        Distance += distanceFlip * 256 - distanceOffset;
    }

    /**
    * Helper methode om de tijd te berekenen
    * Databyte van tijd flipt na 255
    * 1 tijdseenheid = 0.25 seconden vanuit datapakketje
    */
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

    public override string ToString()
    {
        return $"{Speed} {Distance} {Watt} {Time} {Rpm} {HeartRate}";
    }
}