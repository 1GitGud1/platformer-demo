// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/FOVshader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Alpha("Alpha", Range(0.0, 1.0)) = 1.0
        _MainTex("Texture", 2D) = "white" {}
        _GradientRadius ("Gradient Radius", Float) = 1.0
        _FadeStart ("Fade Start", Float) = 0.8
    }
    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha

        Lighting Off
        ZWrite Off
        Pass
        {
            Cull Off
            ColorMask 0
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
        
        }

        
        CGPROGRAM
        #pragma surface surf NoLighting alpha 

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
            return fixed4(s.Albedo, s.Alpha);
        }

        sampler2D _MainTex;
        half4 _Color;
        float _Alpha;

        struct Input {
            float2 uv_MainTex;
            float color : COLOR;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            // float d = distance(_Center, IN.worldPos);
            float d = IN.uv_MainTex.y;
            d = step(0.9, d);
            o.Albedo = _Color; // 1 = (1,1,1,1) = white
            o.Alpha = 0 + saturate((IN.uv_MainTex.y-0.9)*5)*d;
        }
        ENDCG
        
    }
}