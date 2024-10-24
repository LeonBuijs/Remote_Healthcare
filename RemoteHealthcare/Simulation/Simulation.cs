namespace Simulation;

public class Simulation
{
    public int speed { get; set; }
    public double time { get; set; }
    public int watt { get; set; }
    public int rpm { get; set; }
    public int heartRate { get; set; }
    public int distance { get; set; }
    public int counter { get; set; }

    public Simulation(int speed, int watt, int rpm, int heartRate)
    {
        this.speed = speed;
        this.watt = watt;
        this.rpm = rpm;
        this.heartRate = heartRate;
        time = 0;
        distance = 0;
        counter = 0;
        Thread thread = new Thread(UpdateTime);
        thread.Start();
    }

    public byte[] GenerateData()
    {
        byte[] data = null;
        switch (counter)
        {
            case 0:
                // Bikedata 25
                data =
                [
                    0xA4, 0x09, 0x4E, 0x05, 0x19, 0x00, (byte)rpm, 0x00, 0x00, (byte)watt, 0x00, 0x00,
                    0x00
                ];
                break;
            case 1:
                // Bikedata 16
                CalculateDistance();
                data =
                [
                    0xA4, 0x09, 0x4E, 0x05, 0x10, 0x00, (byte)time, (byte)distance, 0x00,
                    (byte)speed, 0x00,
                    0x00, 0x00
                ];
                break;
            case 2:
                // Hartslag
                data = [16, (byte)heartRate, 0x00, 0x00];
                counter = -1;
                break;
            default:
                counter = -1;
                break;
        }

        counter++;
        return data;
    }

    private void CalculateDistance()
    {
        distance = (int)(speed / 3.6 * time) % 255;
    }

    private void UpdateTime()
    {
        while (true)
        {
            Thread.Sleep(250);
            time = (time + 1) % 255;
        }
    }
}