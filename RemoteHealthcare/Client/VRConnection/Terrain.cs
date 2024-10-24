using System.Net.Sockets;
using System.Text.Json.Nodes;

namespace VRConnection;

public class Terrain : VREngine
{
    /**
     * Methode die een terrein aanmaakt van 256 x 256 met een bepaalde hoogte.
     */
    private static void createTerrain()
    {
        // Heuvellandschap
        GenerateTerrain();
        
        int[] heights = new int[256 * 256];


        // SendThroughTunnel("scene/terrain/add", new
        // {
        //     size = new int[] { 256, 256 },
        //     heights = heights
        // });

        RecievePacket();
    }
    
    /**
     * Methode die een node aanmaakt voor een terrein, hierdoor kan het terrein weergegeven worden in de simulator.
     */
    public static string CreateNodeForTerrain()
    {
        createTerrain();
        SendThroughTunnel("scene/node/add", new
        {
            name = "Terrain",
            components = new
            {
                transform = new
                {
                    position = new[] { -25, 0, -50 },
                    scale = 1,
                    rotation = new[] { 0, 0, 0 }
                },
                terrain = new
                {
                    smoothnormals = true
                }
            }
        });

        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket());
        return jsonObject["data"]["data"]["data"]["uuid"].ToString();
    }

    public static void AddLayerToTerrain(string uuid)
    {
        SendThroughTunnel("scene/node/addlayer", new
        {
            id = uuid,
            diffuse = "data/NetworkEngine/textures/grass/grass_green_d.jpg",
            normal = "data/NetworkEngine/textures/grass_normal.png",
            minHeight = 0,
            maxHeight = 10,
            fadeDist = 1
        });
        RecievePacket();
    }
    
    private static void GenerateTerrain()
    {
        // Create terrain:
        int terrainSize = 256;
        float[,] heights = new float[terrainSize, terrainSize];
        for (int x = 0; x < terrainSize; x++)
        for (int y = 0; y < terrainSize; y++)
            heights[x, y] = 2 + (float)(Math.Cos(x / 5.0) + Math.Cos(y / 5.0));
        SendThroughTunnel("scene/terrain/add", new
        {
            size = new[] { terrainSize, terrainSize },
            heights = heights.Cast<float>().ToArray()
        });
    }
}