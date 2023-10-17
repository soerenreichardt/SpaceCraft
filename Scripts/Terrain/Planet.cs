using Terrain.Height;
using Terrain.Height.SebastianLague;
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
        public Material material;
        
        [HideInInspector]
        public bool earthTerrainSettingsFoldout;
        
        private readonly TerrainQuadTree[] planetSides = new TerrainQuadTree[6];

        private MeshGenerator meshGenerator;
        private float planetDiameter;
        
        // Start is called before the first frame update
        void Start()
        {
            meshGenerator = new MeshGenerator(new EarthTerrainNoiseEvaluator(earthTerrainSettings), earthTerrainSettings);
            planetDiameter = Mathf.Pow(2, earthTerrainSettings.planetSize) * SCALE;
            
            InitializePlanetSides();
        }

        private void InitializePlanetSides()
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
                    material,
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

        // Update is called once per frame
        void Update()
        {
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

        private void LateUpdate() {
            meshGenerator.Consume();
        }
    }
}