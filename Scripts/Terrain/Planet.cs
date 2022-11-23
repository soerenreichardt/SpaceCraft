using PostProcessing;
using Terrain.Color;
using Terrain.Height;
using Terrain.Mesh;
using UnityEngine;

namespace Terrain
{
    public class Planet : MonoBehaviour
    {
        public const float SCALE = 0.1f;

        private static readonly string[] DirectionNames = { "up", "down", "left", "right", "front", "back" };
        private static readonly Vector3[] Directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
    
        public EarthTerrainSettings earthTerrainSettings;
        public ColorSettings colorSettings;
        public OceanEffectSettings oceanEffectSettings;

        private MeshGenerator meshGenerator;
        private ColorGenerator colorGenerator;
        
        private readonly TerrainQuadTree[] planetSides = new TerrainQuadTree[6];

        [HideInInspector]
        public bool colorSettingsFoldout;
        [HideInInspector] 
        public bool earthTerrainSettingsFoldout;
        [HideInInspector]
        public bool oceanEffectSettingsFoldout;

        private Light directionalLight;
        
        // Start is called before the first frame update
        void Start()
        {
            meshGenerator = new MeshGenerator(new EarthTerrainNoiseEvaluator(earthTerrainSettings), earthTerrainSettings);
            
            var planetDiameter = Mathf.Pow(2, earthTerrainSettings.planetSize) * SCALE;
            InitializeColorGenerator(planetDiameter);
            InitializeOceanEffect(planetDiameter);
            InitializePlanetSides(planetDiameter);
        }

        private void InitializePlanetSides(float planetDiameter)
        {
            for (int i = 0; i < 6; i++)
            {
                var transformCache = transform;
                var planetSide = new TerrainQuadTree(
                    transformCache.position,
                    planetDiameter,
                    Directions[i],
                    3.0f,
                    earthTerrainSettings.planetSize,
                    colorSettings.material,
                    meshGenerator
                );
                planetSide.terrain.transform.parent = transformCache;
                planetSide.terrain.name = DirectionNames[i];
                planetSides[i] = planetSide;
            }

            planetSides[0].neighbors = new[] {planetSides[2], planetSides[4], planetSides[3], planetSides[5]};
            planetSides[1].neighbors = new[] {planetSides[3], planetSides[4], planetSides[2], planetSides[5]};
            planetSides[2].neighbors = new[] {planetSides[4], planetSides[0], planetSides[5], planetSides[1]};
            planetSides[3].neighbors = new[] {planetSides[5], planetSides[0], planetSides[4], planetSides[1]};
            planetSides[4].neighbors = new[] {planetSides[3], planetSides[0], planetSides[2], planetSides[1]};
            planetSides[5].neighbors = new[] {planetSides[2], planetSides[0], planetSides[3], planetSides[1]};
        }

        private void InitializeColorGenerator(float planetDiameter)
        {
            colorGenerator = new ColorGenerator(colorSettings);
            colorSettings.material.SetFloat("_PlanetRadius", planetDiameter);
            colorSettings.material.SetFloat("_InversePlanetRadius", 1.0f / planetDiameter);
        }

        // Update is called once per frame
        void Update()
        {
            SetVaryingOceanEffectProperties();
            for (var i=0; i<6; i++) 
            {
                planetSides[i].Update();
            }
        }

        public void OnTerrainSettingsUpdated()
        {
            foreach (var planetSide in planetSides)
            {
                planetSide.RecomputeTerrain();
            }
        }

        public void OnColorSettingsUpdated()
        {
            colorGenerator.UpdateColors();
        }

        public void OnOceanEffectSettingsUpdated()
        {
            oceanEffectSettings.SetProperties();
        }
        
        private void LateUpdate() {
            meshGenerator.Consume();
        }

        private void InitializeOceanEffect(float planetDiameter)
        {
            if (Camera.main != null)
                Camera.main.gameObject.GetComponent<PostProcessingEffects>().oceanEffect = oceanEffectSettings.material;

            oceanEffectSettings.SetProperties();
            
            directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
            OceanEffectSettings.FixedData data;
            data.oceanRadius = planetDiameter;
            data.planetScale = 1 / SCALE;
            oceanEffectSettings.SetFixedData(data);
        }

        private void SetVaryingOceanEffectProperties()
        {
            OceanEffectSettings.UpdatableData data;
            data.planetPosition = transform.position;
            data.directionToSun = -directionalLight.transform.forward;
            oceanEffectSettings.SetUpdatableData(data);
        }
    }
}