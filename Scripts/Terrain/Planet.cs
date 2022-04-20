using Noise;
using Terrain;
using UnityEngine;

public class Planet : MonoBehaviour
{

    public static int PLANET_SIZE = 8;
    public static float SCALE = 0.1f;

    private static string[] directionNames = { "up", "down", "left", "right", "front", "back" };
    private static Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
    
    public Material material;
    public NoiseSettings noiseSettings;
    
    public bool recomputeTerrain;

    private MeshGenerator meshGenerator;
    private TerrainQuadTree[] planetSides = new TerrainQuadTree[6];

    // Start is called before the first frame update
    void Start()
    {
        meshGenerator = new MeshGenerator(NoiseFilterFactory.CreateNoiseFilter(noiseSettings));
        for (int i=0; i<6; i++) 
        {
            var planetSide = new TerrainQuadTree(
                transform.position, 
                Mathf.Pow(2, PLANET_SIZE) * SCALE, 
                directions[i],
                3.0f,
                PLANET_SIZE, 
                material,
                meshGenerator
            );
            planetSide.terrain.transform.parent = transform;
            planetSide.terrain.name = directionNames[i];
            planetSides[i] = planetSide;
        }
        
        planetSides[0].neighbors = new[] {planetSides[2], planetSides[4], planetSides[3], planetSides[5]};
        planetSides[1].neighbors = new[] {planetSides[3], planetSides[4], planetSides[2], planetSides[5]};
        planetSides[2].neighbors = new[] {planetSides[4], planetSides[0], planetSides[5], planetSides[1]};
        planetSides[3].neighbors = new[] {planetSides[5], planetSides[0], planetSides[4], planetSides[1]};
        planetSides[4].neighbors = new[] {planetSides[3], planetSides[0], planetSides[2], planetSides[1]};
        planetSides[5].neighbors = new[] {planetSides[2], planetSides[0], planetSides[3], planetSides[1]};
    }

    // Update is called once per frame
    void Update()
    {
        if (recomputeTerrain)
        {
            foreach (var planetSide in planetSides)
            {
                planetSide.recomputeTerrain();
            }

            recomputeTerrain = false;
        }
        for (int i=0; i<6; i++) 
        {
            planetSides[i].update();
        }
    }
    
    private void LateUpdate() {
        meshGenerator.consume();
    }
}