using System.Net.Sockets;
using System.Text.Json.Nodes;

namespace VRConnection;

public class Terrain : VREngine
{
    /**
     * Methode die een terrein aanmaakt van 256 x 256 met een bepaalde hoogte.
     */
    private static void createTerrain(NetworkStream stream)
    {
        // Heuvellandschap

        // int[] terrainMap = new int[256 * 256];
        // GenerateTerrain(256, 256, terrainMap);
        // string jsonString = ConvertArrayToJson(terrainMap);


        int[] heights = new int[256 * 256];

        SendThroughTunnel(stream, "scene/terrain/add", new
        {
            size = new int[] { 256, 256 },
            heights = heights
        });

        RecievePacket(stream);
    }
    
    /**
     * Methode die een node aanmaakt voor een terrein, hierdoor kan het terrein weergegeven worden in de simulator.
     */
    public static string CreateNodeForTerrain(NetworkStream stream)
    {
        createTerrain(stream);
        SendThroughTunnel(stream, "scene/node/add", new
        {
            name = "Terrain",
            components = new
            {
                transform = new
                {
                    position = new[] { -100, 0, -50 },
                    scale = 1,
                    rotation = new[] { 0, 0, 0 }
                },
                terrain = new
                {
                    smoothnormals = true
                }
            }
        });

        JsonObject jsonObject = (JsonObject)JsonObject.Parse(RecievePacket(stream));
        return jsonObject["data"]["data"]["data"]["uuid"].ToString();
    }

    public static void AddLayerToTerrain(NetworkStream stream, string uuid)
    {
        SendThroughTunnel(stream, "scene/node/addlayer", new
        {
            id = uuid,
            diffuse = "data/NetworkEngine/textures/grass_diffuse.png",
            normal = "data/NetworkEngine/textures/grass_normal.png",
            minHeight = 0,
            maxHeight = 10,
            fadeDist = 1
        });
        RecievePacket(stream);
    }
    
    // private static void GenerateTerrain(int width, int height, int[] terrainMap)
    // {
    //     float scale = 20f;  // Bepaalt hoe "heuvelachtig" het terrein is
    //     int maxHeight = 20; // Maximale hoogte van het terrein
    //
    //     for (int y = 0; y < height; y++)
    //     {
    //         for (int x = 0; x < width; x++)
    //         {
    //             float sampleX = x / scale;
    //             float sampleY = y / scale;
    //             float noiseValue = PerlinNoise(sampleX, sampleY);
    //             
    //             // Schaal de noise waarde naar een geheel getal voor het terrein
    //             terrainMap[y * width + x] = (int)((noiseValue + 1) * (maxHeight / 2)); // Zorgt ervoor dat de minimale waarde 0 is
    //         }
    //     }
    // }
    //
    // // Simpele noise functie
    // private static float PerlinNoise(float x, float y)
    // {
    //     return (float)(Math.Sin(x) + Math.Sin(y)) / 2;  // De waarden liggen tussen -1 en 1
    // }
    //
    // private static string ConvertArrayToJson(int[] array)
    // {
    //     StringBuilder jsonBuilder = new StringBuilder();
    //     jsonBuilder.Append("[");
    //     
    //     for (int i = 0; i < array.Length; i++)
    //     {
    //         jsonBuilder.Append(array[i]);
    //         if (i < array.Length - 1)
    //         {
    //             jsonBuilder.Append(",");
    //         }
    //     }
    //     
    //     jsonBuilder.Append("]");
    //     return jsonBuilder.ToString();
    // }
}