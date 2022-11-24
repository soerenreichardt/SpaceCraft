using UnityEngine;

namespace PostProcessing
{
    [CreateAssetMenu]
    public class OceanEffectSettings : 
        ScriptableObject,
        IShaderSettings<OceanEffectSettings.FixedData, OceanEffectSettings.UpdatableData>
    {
        private static readonly int OceanCentrePropertyLocation = Shader.PropertyToID("oceanCentre");
        private static readonly int DirToSunPropertyLocation = Shader.PropertyToID("dirToSun");

        public Material material;
        
        [Header("Water color")]
        public Color colorA;
        public Color colorB;

        public float depthMultiplier = 10.0f;
        public float alphaMultiplier = 70.0f;

        [Header("Waves")] 
        public Texture2D waveNormalA;
        public Texture2D waveNormalB;
        public Color specularColor = Color.white;
        [Range(0,1)]
        public float smoothness;
        [Range(0,1)]
        public float waveStrength;
        [Range(0,1)]
        public float waveSpeed;

        public float waveScale = 15.0f;

        public void SetProperties()
        {
            material.SetColor("colA", colorA);
            material.SetColor("colB", colorB);
            
            material.SetFloat("depthMultiplier", depthMultiplier);
            material.SetFloat("alphaMultiplier", alphaMultiplier);

            material.SetTexture("waveNormalA", waveNormalA);
            material.SetTexture("waveNormalB", waveNormalB);
            
            material.SetColor("specularCol", specularColor);
            
            material.SetFloat("smoothness", smoothness);
            material.SetFloat("waveStrength", waveStrength);
            material.SetFloat("waveSpeed", waveSpeed);
            material.SetFloat("waveNormalScale", waveScale);
        }

        public void SetFixedData(FixedData data)
        {
            material.SetFloat("oceanRadius", data.oceanRadius);
            material.SetFloat("planetScale", data.planetScale);
        }

        public void SetUpdatableData(UpdatableData data)
        {
            material.SetVector(OceanCentrePropertyLocation, data.planetPosition);
            material.SetVector(DirToSunPropertyLocation, data.directionToSun);
        }

        public struct FixedData
        {
            public float oceanRadius;
            public float planetScale;
        }
        
        public struct UpdatableData
        {
            public Vector3 planetPosition;
            public Vector3 directionToSun;
        }
    }
}