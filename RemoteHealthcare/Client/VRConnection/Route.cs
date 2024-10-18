using System.Net.Sockets;
using System.Text.Json.Nodes;

namespace VRConnection;

public class Route : VREngine
{
    /**
     * Methode die de snelheid aanpast waarmee een node een route volgt,
     * je geeft de uuid van de node mee met de snelheid die je wilt instellen.
     */
    public static void ChangeFollowRouteSpeed(NetworkStream stream, string uuidBike, double speed)
    {
        SendThroughTunnel(stream, "route/follow/speed", new { node = uuidBike, speed = speed });
        RecievePacket(stream);
    }

    /**
     * Methode die een node een route laat volgen,
     * je geeft de uuid van de route mee en de uuid van de node die deze route moet volgen.
     */
    public static void FollowRoute(NetworkStream stream, string uuidRoute, string uuidBike)
    {
        SendThroughTunnel(stream, "route/follow", new
        {
            route = uuidRoute,
            node = uuidBike,
            speed = 10.0,
            offset = 0.0,
            rotate = "XZ",
            smoothing = 1.0,
            followHeight = false,
            rotateOffset = new[] { 0, 0, 0 },
            positionOffset = new[] { 0, 0, 0 }
        });

        RecievePacket(stream);
    }

    /**
     * Methode die een road aanmaakt op een route, je geeft de uuid van de route mee.
     */
    public static void CreateRoad(NetworkStream stream, string uuidRoute)
    {
        SendThroughTunnel(stream, "scene/road/add", new { route = uuidRoute });
        RecievePacket(stream);
    }

    /**
     * Methode die een route aanmaakt door middel van een aantal punten.
     */
    public static string CreateRoute(NetworkStream stream)
    {
        SendThroughTunnel(stream, "route/add", new
        {
            nodes = new[]
            {
                new { pos = new[] { 0, 0, 0 }, dir = new[] { 5, 0, 0 }},
                new { pos = new[] { 40, 0, 0 }, dir = new[] { 5, 0, 2 }},
                new { pos = new[] { 75, 0, 20 }, dir = new[] { 3, 0, 3 }},
                new { pos = new[] { 100, 0, 50 }, dir = new[] { 0, 0, 5 }},
                new { pos = new[] { 100, 0, 100 }, dir = new[] { -3, 0, 3 }},
                new { pos = new[] { 75, 0, 125 }, dir = new[] { -5, 0, 2 }},
                new { pos = new[] { 40, 0, 140 }, dir = new[] { -5, 0, -2 }},
                new { pos = new[] { 0, 0, 125 }, dir = new[] { -3, 0, -3 }},
                new { pos = new[] { -25, 0, 100 }, dir = new[] { 0, 0, -5 }},
                new { pos = new[] { -25, 0, 50 }, dir = new[] { 3, 0, -3 }},
                new { pos = new[] { 0, 0, 25 }, dir = new[] { 5, 0, 0 }}
            }
        });

        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        return jsonObject["data"]["data"]["data"]["uuid"].ToString();
    }
}