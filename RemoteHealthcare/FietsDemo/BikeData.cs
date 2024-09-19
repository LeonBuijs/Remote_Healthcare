using System;

namespace FietsDemo
{
    public class BikeData
    {
        private bool initialized;

        public int speed { get; set; }
        public int watt { get; set; }
        public int rpm { get; set; }
        public int heartRate { get; set; }

        //attributen voor tijd
        public double time { get; set; } //todo mogelijk implementeren nog
        private double lastTime;
        private int timeFlip;
        private double timeOffset;

        //attributen voor afstanden
        public int distance { get; set; }
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
                    heartRate = int.Parse(split[1], System.Globalization.NumberStyles.HexNumber);
                    break;
                case 6:
                    //todo andere data van hartslagmeter, mogelijk nog extra data implementeren.
                    break;
                case 13:
                    changeBikeData(split);
                    break;
            }
        }

        private void changeBikeData(string[] split)
        {
            var PageNumber = int.Parse(split[4], System.Globalization.NumberStyles.HexNumber);

            switch (PageNumber)
            {
                case 16:
                    speed = int.Parse(split[9], System.Globalization.NumberStyles.HexNumber);

                    calculateTime(split);

                    CalculateDistance(split);

                    if (!initialized)
                    {
                        time = 0;
                        distanceOffset = distance;
                        initialized = true;
                    }

                    break;
                case 25:
                    rpm = int.Parse(split[6], System.Globalization.NumberStyles.HexNumber);
                    watt = int.Parse(split[9], System.Globalization.NumberStyles.HexNumber);
                    break;
            }
        }

        private void CalculateDistance(string[] split)
        {
            //distance flipt na 255
            distance = int.Parse(split[7], System.Globalization.NumberStyles.HexNumber);
            if (lastDistance > 200 && distance < 100)
            {
                distanceFlip++;
            }

            lastDistance = distance;

            distance += distanceFlip * 256 - distanceOffset;
        }

        //todo tijd goed laten weergeven met juiste offset en waardes
        private void calculateTime(string[] split)
        {
            //time flipt na 255
            var tempTime = int.Parse(split[6], System.Globalization.NumberStyles.HexNumber);

            var difference = tempTime - lastTime;

            if (difference < 0)
            {
                time += 256 + difference;
            }
            else
            {
                time += difference * 0.25;
            }

            if (lastTime > 200 && tempTime < 100)
            {
                timeFlip++;
            }

            lastTime = tempTime;
        }
    }
}