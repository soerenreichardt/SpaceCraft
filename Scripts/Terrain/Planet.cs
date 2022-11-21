using Terrain;
using Terrain.Color;
using UnityEngine;

public class Planet : MonoBehaviour
{

    public static float SCALE = 0.1f;

    private static string[] directionNames = { "up", "down", "left", "right", "front", "back" };
    private static Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
    
    public TerrainSettings terrainSettings;
    public ColorSettings colorSettings;
    
    private MeshGenerator meshGenerator;
    private TerrainQuadTree[] planetSides = new TerrainQuadTree[6];

    private ColorGenerator colorGenerator;
    
    [HideInInspector]
    public bool terrainSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;
    
    // Start is called before the first frame update
    void Start()
    {
        var planetDiameter = Mathf.Pow(2, terrainSettings.planetSize) * SCALE;
        
        meshGenerator = new MeshGenerator(terrainSettings);
        colorSettings.material.SetFloat("_PlanetRadius", planetDiameter);
        colorSettings.material.SetFloat("_InversePlanetRadius", 1.0f / planetDiameter);

        colorGenerator = new ColorGenerator(colorSettings);
        
        for (int i=0; i<6; i++) 
        {
            var planetSide = new TerrainQuadTree(
                transform.position, 
                planetDiameter, 
                directions[i],
                3.0f,
                terrainSettings.planetSize, 
                colorSettings.material,
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
        for (int i=0; i<6; i++) 
        {
            planetSides[i].update();
        }
    }

    public void OnTerrainSettingsUpdated()
    {
        foreach (var planetSide in planetSides)
        {
            planetSide.recomputeTerrain();
        }
    }

    public void OnColorSettingsUpdated()
    {
        colorGenerator.UpdateColors();
    }
    
    private void LateUpdate() {
        meshGenerator.consume();
    }
}