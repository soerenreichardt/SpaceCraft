using UnityEngine;

public class Planet : MonoBehaviour
{

    public static int PLANET_SIZE = 8;
    public static float SCALE = 0.1f;

    public Material material;

    TerrainQuadTree[] planetSides = new TerrainQuadTree[6];
    
    private static string[] directionNames = { "up", "down", "left", "right", "front", "back" };
    private static Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i<6; i++) 
        {
            TerrainQuadTree planetSide = planetSides[i];
            planetSide = new TerrainQuadTree(transform.position, Mathf.Pow(2, PLANET_SIZE) * SCALE, directions[i], 3.0f,
                PLANET_SIZE, material);
            planetSide.terrain.transform.parent = transform;
            planetSide.terrain.name = directionNames[i];
            planetSides[i] = planetSide;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i=0; i<6; i++) 
        {
            planetSides[i].update();
        }
    }
}
