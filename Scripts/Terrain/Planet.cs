using Terrain;
using Terrain.Color;
using UnityEngine;

public class Planet : MonoBehaviour
{

    public static float SCALE = 0.1f;

    private static string[] directionNames = { "up", "down", "left", "right", "front", "back" };
    private static Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
    
    public LayeredTerrainSettings layeredTerrainSettings;
    public EarthTerrainSettings earthTerrainSettings;
    public ColorSettings colorSettings;

    public Material oceanEffect;
    
    private MeshGenerator meshGenerator;
    private TerrainQuadTree[] planetSides = new TerrainQuadTree[6];

    private ColorGenerator colorGenerator;
    
    [HideInInspector]
    public bool terrainSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;
    [HideInInspector] 
    public bool earthTerrainSettingsFoldout;

    private Light directionalLight;
    
    // Start is called before the first frame update
    void Start()
    {
        var planetDiameter = Mathf.Pow(2, earthTerrainSettings.planetSize) * SCALE;
        
        meshGenerator = new MeshGenerator(new EarthTerrainNoiseEvaluator(earthTerrainSettings), earthTerrainSettings);
        colorGenerator = new ColorGenerator(colorSettings);
        directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
        setFixedShaderProperties(planetDiameter);
        
        for (int i=0; i<6; i++) 
        {
            var transformCache = transform;
            var planetSide = new TerrainQuadTree(
                transformCache.position, 
                planetDiameter, 
                directions[i],
                3.0f,
                earthTerrainSettings.planetSize, 
                colorSettings.material,
                meshGenerator
            );
            planetSide.terrain.transform.parent = transformCache;
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
        setVaryingShaderProperties();
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

    private void setFixedShaderProperties(float planetDiameter)
    {
        colorSettings.material.SetFloat("_PlanetRadius", planetDiameter);
        colorSettings.material.SetFloat("_InversePlanetRadius", 1.0f / planetDiameter);
        
        oceanEffect.SetFloat("oceanRadius", planetDiameter);
        oceanEffect.SetFloat("planetScale", planetDiameter / 2.0f);
    }

    private void setVaryingShaderProperties()
    {
        oceanEffect.SetVector("oceanCentre", transform.position);
        oceanEffect.SetVector("dirToSun", -directionalLight.transform.forward);
    }
}