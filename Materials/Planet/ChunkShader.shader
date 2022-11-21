Shader "Custom/ChunkShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _PlanetRadius ("Planet radius", Float) = 1.0
        _InversePlanetRadius ("Inverse planet radius", Float) = 0.1
        _WaterLevel ("Water level", Range(0,1)) = 0.5
        _TerrainColors ("Terrain colors", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        #include "UnityCG.cginc"

        sampler2D _MainTex;
        sampler2D _TerrainColors;

        struct Input
        {
            float2 uv_MainTex;
            float3 localPos;
        };

        half _Glossiness;
        half _Metallic;
        half _PlanetRadius;
        half _InversePlanetRadius;
        half _WaterLevel;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float inverse_lerp(float min, float max, float val);
        
        void vert (inout appdata_full v, out Input o) {
           UNITY_INITIALIZE_OUTPUT(Input,o);
           o.localPos = v.vertex.xyz;
        }
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            float heightPercentage = inverse_lerp(0, 0.1, (length(IN.localPos) - _PlanetRadius) * _InversePlanetRadius);
            fixed4 heightColor = tex2D (_TerrainColors, float2(heightPercentage, 0.0));
            // fixed4 heightColor = fixed4(heightPercentage, heightPercentage, heightPercentage, 1.0);
            o.Albedo = heightColor.xyz;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = heightColor.a;
            
        }

        float inverse_lerp(float min, float max, float val) {
            return (val - min) / (max - min);
        }
        ENDCG
    }
    FallBack "Diffuse"
}