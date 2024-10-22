using System.Net.Sockets;
using System.Text.Json.Nodes;

namespace VRConnection;

public class Route : VREngine
{
    /**
     * Methode die de snelheid aanpast waarmee een node een route volgt,
     * je geeft de uuid van de node mee met de snelheid die je wilt instellen.
     */
    public static void ChangeFollowRouteSpeed(string uuidBike, double speed)
    {
        SendThroughTunnel("route/follow/speed", new { node = uuidBike, speed = speed });
        RecievePacket();
    }

    /**
     * Methode die een node een route laat volgen,
     * je geeft de uuid van de route mee en de uuid van de node die deze route moet volgen.
     */
    public static void FollowRoute(string uuidRoute, string uuidBike)
    {
        SendThroughTunnel("route/follow", new
        {
            route = uuidRoute,
            node = uuidBike,
            speed = 10.0,
            offset = 0.0,
            rotate = "XZ",
            smoothing = 1.0,
            followHeight = true,
            rotateOffset = new[] { 0, 0, 0 },
            positionOffset = new[] { 0, 0, 0 }
        });

        RecievePacket();
    }

    /**
     * Methode die een road aanmaakt op een route, je geeft de uuid van de route mee.
     */
    public static void CreateRoad(string uuidRoute)
    {
        SendThroughTunnel("scene/road/add", new { route = uuidRoute });
        RecievePacket();
    }

    /**
     * Methode die een route aanmaakt door middel van een aantal punten.
     */
    public static string CreateRoute()
    {
        SendThroughTunnel("route/add", new
        {
            nodes = new[]
            {
                // Bocht 1
                new { pos = new[] { 0, 0, 0 }, dir = new[] { 0, 0, -20 }},
                new { pos = new[] { 10, 0, -10 }, dir = new[] { 20, 0, 0 }},
                new { pos = new[] { 20, 0, 0 }, dir = new[] { 0, 0, 20 }},
                new { pos = new[] { 20, 0, 15 }, dir = new[] { 0, 0, 20 }},
                // Bocht 2
                new { pos = new[] { 30, 0, 35 }, dir = new[] { 0, 0, 20 }},
                new { pos = new[] { 15, 0, 50 }, dir = new[] { 0, 0, 20 }},
                // Bocht 3
                new { pos = new[] { 30, 0, 60 }, dir = new[] { 20, 0, -20 }},
                // Bocht 4
                new { pos = new[] { 50, 0, 42 }, dir = new[] { 20, 0, -20 }},
                // Bocht 5
                new { pos = new[] { 90, 0, 30 }, dir = new[] { 20, 0, -20 }},
                // Bocht 6
                new { pos = new[] { 110, 0, 0 }, dir = new[] { 20, 0, -20 }}, 
                new { pos = new[] { 150, 0, -30 }, dir = new[] { 80, 0, -50 }}, 
                
                // Bocht 7
                new { pos = new[] { 200, 0, 10 }, dir = new[] { 0, 0, 20 }}, 
                
                // Bocht 8
                new { pos = new[] { 200, 0, 60 }, dir = new[] { -10, 0, 20 }}, 
                new { pos = new[] { 160, 0, 80 }, dir = new[] { -20, 0, 0 }}, 
                
                // Bocht 9
                new { pos = new[] { 120, 0, 70 }, dir = new[] { -20, 0, -20 }}, 
                new { pos = new[] { 130, 0, 50 }, dir = new[] { 20, 0, -20 }}, 
                
                // Bocht 10
                new { pos = new[] { 150, 0, 35 }, dir = new[] { 20, 0, -20 }},
                new { pos = new[] { 135, 0, 15 }, dir = new[] { -20, 0, 0 }},
                new { pos = new[] { 90, 0, 40 }, dir = new[] { -20, 0, 10 }},
                
                // Bocht 11
                new { pos = new[] { 60, 0, 80 }, dir = new[] { -10, 0, 20 }},
                
                // Bocht 12
                new { pos = new[] { 45, 0, 72 }, dir = new[] { -10, 0, 0 }},
                new { pos = new[] { 40, 0, 85 }, dir = new[] { 20, 0, 20 }},
                
                // Bocht 13
                new { pos = new[] { 85, 0, 135 }, dir = new[] { 10, 0, 20 }},
                new { pos = new[] { 60, 0, 155 }, dir = new[] { -20, 0, 15 }},
                
                // Bocht 14
                new { pos = new[] { 20, 0, 155 }, dir = new[] { -10, 0, 0 }},
                new { pos = new[] { 0, 0, 130 }, dir = new[] { 0, 0, -20 }},
            }
        });

        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket());
        return jsonObject["data"]["data"]["data"]["uuid"].ToString();
    }
}