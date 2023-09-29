using Terrain.Height.SebastianLague;
using UnityEngine;

namespace Terrain
{
    public class EarthPlanet : MonoBehaviour
    {
        public EarthTerrainSettings earthTerrainSettings;
        public Material material;
        
        [HideInInspector] 
        public bool earthTerrainSettingsFoldout;
        
        private Planet planet;
        
        void Start()
        {
            this.earthTerrainSettings = Instantiate(earthTerrainSettings);
            this.planet = new Planet(new EarthTerrainNoiseEvaluator(earthTerrainSettings), earthTerrainSettings,
                material, transform);
        }
        
        public void OnTerrainSettingsUpdated()
        {
            planet.RecomputeTerrain();
        }
        
        void Update()
        {
            planet.Update();
        }
        
        void LateUpdate()
        {
            planet.LateUpdate();
        }
    }
}