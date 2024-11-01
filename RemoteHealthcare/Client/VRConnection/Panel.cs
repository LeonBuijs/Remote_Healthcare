using System.Net.Sockets;
using System.Text.Json.Nodes;

namespace VRConnection;

public class Panel : VREngine
{
    private static int[] black = [0, 0, 0, 1];
    /**
     * Methode om de naam aan te passen op het panel, je geeft de uuid van het panel mee en de naam.
     */
    public static void ChangeNamePanel(string uuidPanel, string name)
    {
        DrawTextOnPanel(name, uuidPanel,new[] { 10, 30 }, 30, black);
    }
    
    public static void SetDataText(string uuidPanel)
    {
        DrawTextOnPanel("Snelheid | Hartslag | Tijdsduur | Afstand", uuidPanel,new[] { 10, 20 }, 18, black);
    }

    /**
     * Methode om de data aan te passen op het panel, je geeft de uuid van het panel mee en de data.
     */
    public static void ChangeDataPanel(string uuidPanel, int speed, int heartRate, string time, int distance)
    {
        // TODO: Mooier uitlijnen voor elke waarde
        DrawTextOnPanel("" + speed + "           " + heartRate + "          " + time + "         " + distance, 
            uuidPanel,new[] {25, 10 }, 18, black);
    }
    
    /**
     * Methode om de afstand aan te passen op het panel, je geeft de uuid van het panel mee en de afstand.
     */
    public static void ChangeChatsPanel(string uuidPanel, List<string> chats, int[] color)
    {
        if (chats.Count >= 8)
        {
            for (int i = 5; i > 0; i--)
            {
                DrawTextOnPanel(chats[chats.Count - i], uuidPanel, new[] { 10, (8 - i) * 20 + 60 }, 20, color);
            }
        }
        else
        {
            for (int i = chats.Count; i > 0; i--)
            {
                DrawTextOnPanel(chats[chats.Count - i], uuidPanel, new[] { 10, (chats.Count - i) * 20 + 60 }, 20, color);
            }
        }
    }
    
    /**
     * Methode om meegegeven tekst weer te geven op het meegegeven panel.
     */
    private static void DrawTextOnPanel(string text, string uuidPanel, int[] position, int size, int[] color)
    {
        SendThroughTunnel("scene/panel/drawtext", new
        {
            id = uuidPanel,
            text = text,
            position = position,
            size = size,
            color = new[] { 0, 0, 0, 1 }
        });
        RecievePacket();
    }

    /**
     * Methode om het meegegeven panel te clearen.
     */
    public static void ClearPanel(string uuid)
    {
        SendThroughTunnel("scene/panel/clear", new { id = uuid });
        RecievePacket();
    }

    /**
     * Methode die de buffer wisselt van het panel die je meegeeft,
     * dit zorgt er eigenlijk voor dat het panel geupdate wordt.
     */
    public static void SwapPanel(string uuid)
    {
        SendThroughTunnel("scene/panel/swap", new
        {
            id = uuid
        });
        RecievePacket();
    }

    /**
     * Methode die een node maakt waardoor het panel weergegeven kan worden.
     */
    public static string CreateNodeForPanel()
    {
        SendThroughTunnel("scene/node/add", new
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

        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket());
        return jsonObject["data"]["data"]["data"]["uuid"].ToString();
    }
    
    /**
     * Methode die het panel koppelt aan de bike zodat het panel in beeld staat,
     * je moet de uuid van het panel en de bike meegeven.
     */
    public static void AttachPanelToBike(string uuidPanel, string uuidBike, double[] position)
    {
        SendThroughTunnel("scene/node/update", new
        {
            id = uuidPanel,
            parent = uuidBike,
            transform = new
            {
                position = position,
                scale = 1,
                rotation = new[] { 0, 90, 0 }
            }
        });
        RecievePacket();
    }
}