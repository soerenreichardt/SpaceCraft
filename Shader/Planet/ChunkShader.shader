Shader "Custom/ChunkShader"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,0)
        _PlanetRadius ("Planet radius", Float) = 1.0
        _InversePlanetRadius ("Inverse planet radius", Float) = 0.1
        _WaterLevel ("Water level", Range(0,1)) = 0.5
        _TerrainColors ("Terrain colors", 2D) = "white" {}
        _PoleCaps ("Pole caps", Range(0,1)) = 1.0
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
        #include "../Math.cginc"

        sampler2D _MainTex;
        sampler2D _TerrainColors;

        float4 _Color;

        struct Input
        {
            float2 uv_MainTex;
            float3 localPos;
            float3 normal;
        };

        half _Glossiness;
        half _Metallic;
        half _PlanetRadius;
        half _InversePlanetRadius;
        half _WaterLevel;
        half _PoleCaps;

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
           o.normal = v.normal.xyz;
        }
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float steepness = 1 - dot(IN.normal, normalize(IN.localPos));
            float heightPercentage = inverse_lerp(-0.1, 0.1, (length(IN.localPos) - _PlanetRadius) * _InversePlanetRadius);
            float steepnessWithHeight = saturate((heightPercentage + steepness) * 0.5);
            // steepness += 1 - heightPercentage;
            fixed4 heightColor = tex2D (_TerrainColors, float2(steepnessWithHeight, 0.0));
            heightColor += max(0.0, heightPercentage - _PoleCaps);
            // fixed4 heightColor = fixed4(steepness, steepness, steepness, 1.0);
            // fixed4 heightColor = _Color;
            o.Albedo = heightColor.xyz;
            o.Alpha = heightColor.a;
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}