using System;
using PostProcessing;
using UnityEngine;

namespace Terrain.Color
{
    [CreateAssetMenu]
    public class EarthShadingSettings : 
        ScriptableObject,
        IShaderSettings<EarthShadingSettings.FixedData, EarthShadingSettings.UpdatableData>
    {
        public Shader shader;
        [HideInInspector]
        public Material material;

        [Header("Flat Terrain")]
        public UnityEngine.Color shoreLow;
        public UnityEngine.Color shoreHigh;
        public UnityEngine.Color flatLowA;
        public UnityEngine.Color flatHighA;
        public UnityEngine.Color flatLowB;
        public UnityEngine.Color flatHighB;
        [Range(0, 3)] public float colorBlend = 1.5f;
        [Range(0, 1)] public float blendNoise = 0.3f;
        [Range(0, 0.2f)] public float shoreHeight = 0.05f;
        [Range(0, 0.2f)] public float shoreBlend = 0.03f;
        [Range(0, 1)] public float maxFlatHeight = 0.5f;

        [Header("Steep Terrain")] 
        public UnityEngine.Color steepLow;
        public UnityEngine.Color steepHeight;
        [Range(1, 29)] public float steepBands = 8;
        [Range(-1, 1)] public float bandStrength;

        [Header("Flat to Steep Transition")] 
        [Range(0, 1)] public float steepnessThreshold = 0.5f;
        [Range(0, 0.3f)] public float flatToSteepBlend = 0.1f;
        [Range(0, 0.2f)] public float flatToSteepNoise = 0.1f;

        [Header("Snowy Poles")] 
        public float usePoles;
        public UnityEngine.Color snowColor = UnityEngine.Color.white;
        [Range(0, 1)] public float snowLongitude = 0.8f;
        [Range(0, 0.2f)] public float snowBlend = 0.1f;
        [Range(0, 1)] public float snowSpecular = 1;
        [Range(1, 2)] public float snowHighlight = 1.2f;
        [Range(0, 10)] public float snowNoiseA = 5;
        [Range(0, 10)] public float snowNoiseB = 4;

        [Header("Noise")] 
        public Texture2D noiseTexture;
        public float noiseScale = 1;
        public float noiseScale2 = 1;

        [Header("Other")] 
        public UnityEngine.Color fresnelColor;
        public float fresnelStrengthMin = 2;
        public float fresnelStrengthMax = 5;
        public float fresnelPower = 2;

        public float smoothness = 0.5f;
        public float metallic;

        public Vector3 testParams;

        public float heightMin;
        public float heightMax;
        public float oceanLevel;
        public float bodyScale;

        private void OnEnable()
        {
            material = new Material(shader);
        }

        public void SetProperties()
        {
            material.SetColor("_ShoreLow", shoreLow);
            material.SetColor("_ShoreHigh", shoreHigh);
            material.SetColor("_FlatLowA", flatLowA);
            material.SetColor("_FlatHighA", flatHighA);
            material.SetColor("_FlatLowB", flatLowB);
            material.SetColor("_FlatHighB", flatHighB);
            material.SetFloat("_FlatColBlend", colorBlend);
            material.SetFloat("_FlatColBlendNoise", blendNoise);
            material.SetFloat("_ShoreHeight", shoreHeight);
            material.SetFloat("_ShoreBlend", shoreBlend);
            material.SetFloat("_MaxFlatHeight", maxFlatHeight);

            material.SetColor("_SteepLow", steepLow);
            material.SetColor("_SteepHigh", steepHeight);
            material.SetFloat("_SteepBands", steepBands);
            material.SetFloat("_SteepBandStrength", bandStrength);
 
            material.SetFloat("_SteepnessThreshold", steepnessThreshold);
            material.SetFloat("_FlatToSteepBlend", flatToSteepBlend);
            material.SetFloat("_FlatToSteepNoise", flatToSteepNoise);
 
            material.SetFloat("_UseSnowyPoles", usePoles);
            material.SetColor("_SnowCol", snowColor);
            material.SetFloat("_SnowLongitude", snowLongitude);
            material.SetFloat("_SnowBlend", snowBlend);
            material.SetFloat("_SnowSpecular", snowSpecular);
            material.SetFloat("_SnowHighLight", snowHighlight);
            material.SetFloat("_SnowNoiseA", snowNoiseA);
            material.SetFloat("_SnowNoiseB", snowNoiseB);
 
            material.SetTexture("_NoiseTex", noiseTexture);
            material.SetFloat("_NoiseScale", noiseScale);
            material.SetFloat("_NoiseScale2", noiseScale2);
 
            material.SetColor("_FresnelCol", fresnelColor);
            material.SetFloat("_FresnelStrengthNear", fresnelStrengthMin);
            material.SetFloat("_fresnelStrengthFar", fresnelStrengthMax);
            material.SetFloat("_FresnelPow", fresnelPower);
 
            material.SetFloat("_Glossiness", smoothness);
            material.SetFloat("_Metallilc", metallic);
            
            material.SetVector("_TestParams", testParams);
            
            material.SetFloat("heightMin", heightMin);
            material.SetFloat("heightMax", heightMax);
            material.SetFloat("oceanLevel", oceanLevel);
            material.SetFloat("bodyScale", bodyScale);
        }

        public void SetFixedData(FixedData data)
        {
            heightMin = data.heightMin;
            heightMax = data.heightMax;
            oceanLevel = data.oceanLevel;
        }

        public void SetUpdatableData(UpdatableData data)
        {
            throw new NotImplementedException();
        }

        public struct FixedData
        {
            public float heightMin;
            public float heightMax;
            public float oceanLevel;
        }
        
        public struct UpdatableData {}
    }
}