using System.Net.Sockets;
using System.Text.Json.Nodes;

namespace VRConnection;

public class Panel : VREngine
{
    /**
     * Methode om de naam aan te passen op het panel, je geeft de uuid van het panel mee en de naam.
     */
    public static void ChangeNamePanel(NetworkStream stream, string uuidPanel, string name)
    {
        DrawTextOnPanel(stream, name, uuidPanel,new[] { 10, 30 }, 30);
    }

    /**
     * Methode om de snelheid aan te passen op het panel, je geeft de uuid van het panel mee en de snelheid.
     */
    public static void ChangeSpeedPanel(NetworkStream stream, string uuidPanel, int speed)
    {
        DrawTextOnPanel(stream, "Snelheid: " + speed, uuidPanel,new[] { 10, 60 }, 20);
    }

    /**
     * Methode om het wattage aan te passen op het panel, je geeft de uuid van het panel mee en het wattage.
     */
    public static void ChangeWattPanel(NetworkStream stream, string uuidPanel, int watt)
    {
        DrawTextOnPanel(stream, "Watt: " + watt, uuidPanel,new[] { 10, 80 }, 20);
    }

    /**
     * Methode om de RPM aan te passen op het panel, je geeft de uuid van het panel mee en de RPM.
     */
    public static void ChangeRPMPanel(NetworkStream stream, string uuidPanel, int rpm)
    {
        DrawTextOnPanel(stream, "RPM: " + rpm, uuidPanel,new[] { 10, 100 }, 20);
    }

    /**
     * Methode om de hartslag aan te passen op het panel, je geeft de uuid van het panel mee en de hartslag.
     */
    public static void ChangeHeartRatePanel(NetworkStream stream, string uuidPanel, int heartRate)
    {
        DrawTextOnPanel(stream, "Hartslag: " + heartRate, uuidPanel,new[] { 10, 120 }, 20);
    }

    /**
     * Methode om de tijd aan te passen op het panel, je geeft de uuid van het panel mee en de tijd.
     */
    public static void ChangeTimePanel(NetworkStream stream, string uuidPanel, string time)
    {
        DrawTextOnPanel(stream, "Tijdsduur: " + time, uuidPanel,new[] { 10, 140 }, 20);
    }

    /**
     * Methode om de afstand aan te passen op het panel, je geeft de uuid van het panel mee en de afstand.
     */
    public static void ChangeDistancePanel(NetworkStream stream, string uuidPanel, int distance)
    {
        DrawTextOnPanel(stream, "Afstand: " + distance, uuidPanel,new[] { 10, 160 }, 20);
    }
    
    /**
     * Methode om meegegeven tekst weer te geven op het meegegeven panel.
     */
    private static void DrawTextOnPanel(NetworkStream stream, string text, string uuidPanel, int[] position, int size)
    {
        SendThroughTunnel(stream, "scene/panel/drawtext", new
        {
            id = uuidPanel,
            text = text,
            position = position,
            size = size,
            color = new[] { 0, 0, 0, 1 }
        });
        RecievePacket(stream);
    }

    /**
     * Methode om het meegegeven panel te clearen.
     */
    public static void ClearPanel(NetworkStream stream, string uuid)
    {
        SendThroughTunnel(stream, "scene/panel/clear", new { id = uuid });
        RecievePacket(stream);
    }

    /**
     * Methode die de buffer wisselt van het panel die je meegeeft,
     * dit zorgt er eigenlijk voor dat het panel geupdate wordt.
     */
    public static void SwapPanel(NetworkStream stream, string uuid)
    {
        SendThroughTunnel(stream, "scene/panel/swap", new
        {
            id = uuid
        });
        RecievePacket(stream);
    }

    /**
     * Methode die een node maakt waardoor het panel weergegeven kan worden.
     */
    public static string CreateNodeForPanel(NetworkStream stream)
    {
        SendThroughTunnel(stream, "scene/node/add", new
        {
            name = "Panel",
            components = new
            {
                transform = new
                {
                    position = new[] { 0, 0, 0 },
                    scale = 1,
                    rotation = new[] { 0, 0, 0 },
                },
                panel = new
                {
                    size = new[] { 1, 1 },
                    resolution = new[] { 256, 256 },
                    background = new[] { 1, 1, 1, 1 },
                    castShadow = false
                }
            }
        });

        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        return jsonObject["data"]["data"]["data"]["uuid"].ToString();
    }
    
    /**
     * Methode die het panel koppelt aan de bike zodat het panel in beeld staat,
     * je moet de uuid van het panel en de bike meegeven.
     */
    public static void AttachPanelToBike(NetworkStream stream, string uuidPanel, string uuidBike)
    {
        SendThroughTunnel(stream, "scene/node/update", new
        {
            id = uuidPanel,
            parent = uuidBike,
            transform = new
            {
                position = new[] { -2, 2.4, -2.1 },
                scale = 1,
                rotation = new[] { 0, 90, 0 }
            }
        });
        RecievePacket(stream);
    }
}