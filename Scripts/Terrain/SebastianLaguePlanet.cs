using PostProcessing;
using Terrain.Color;
using Terrain.Height.SebastianLague;
using UnityEngine;

namespace Terrain
{
    public class SebastianLaguePlanet : MonoBehaviour
    {
        public EarthTerrainSettings earthTerrainSettings;
        public EarthShadingSettings shadingSettings;
        public OceanEffectSettings oceanEffectSettings;

        [HideInInspector]
        public bool shadingSettingsFoldout;
        [HideInInspector] 
        public bool earthTerrainSettingsFoldout;
        [HideInInspector]
        public bool oceanEffectSettingsFoldout;

        private Planet planet;
        private Light directionalLight;
        
        // Start is called before the first frame update
        void Start()
        {
            this.earthTerrainSettings = Instantiate(earthTerrainSettings);
            this.shadingSettings = Instantiate(shadingSettings);
            this.oceanEffectSettings = Instantiate(oceanEffectSettings);
            this.planet = new Planet(new EarthTerrainNoiseEvaluator(earthTerrainSettings), earthTerrainSettings,
                shadingSettings.material, transform);
            InitializeEarthShader();
            InitializeOceanEffect();
        }

        // Update is called once per frame
        void Update()
        {
            SetVaryingOceanEffectProperties();
            planet.Update();
        }

        public void OnTerrainSettingsUpdated()
        {
            planet.RecomputeTerrain();
        }

        public void OnShadingSettingsUpdated()
        {
            shadingSettings.SetProperties();
        }

        public void OnOceanEffectSettingsUpdated()
        {
            oceanEffectSettings.SetProperties();
        }
        
        private void LateUpdate()
        {
            planet.LateUpdate();
        }

        private void InitializeEarthShader()
        {
            EarthShadingSettings.FixedData data;
            data.heightMin = 0.0f;
            data.heightMax = planet.Diameter() + planet.Diameter() * Planet.SCALE;
            data.oceanLevel = planet.Diameter();
            shadingSettings.SetFixedData(data);
            shadingSettings.SetProperties();
        }
        
        private void InitializeOceanEffect()
        {
            if (Camera.main != null)
                Camera.main.gameObject.GetComponent<PostProcessingEffects>().oceanEffect = oceanEffectSettings.material;

            oceanEffectSettings.SetProperties();
            
            directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
            OceanEffectSettings.FixedData data;
            data.oceanRadius = planet.Diameter();
            data.planetScale = 1 / Planet.SCALE;
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