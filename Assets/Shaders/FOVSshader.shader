Shader "Custom/FOVSshader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Alpha("Alpha", Range(0.0, 1.0)) = 1.0
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
        Cull Off

        
        CGPROGRAM
        #pragma surface surf NoLighting alpha 

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
            return fixed4(s.Albedo, s.Alpha);
        }

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
            o.Alpha = 0.2;//0 + 1-saturate((IN.uv_MainTex.y-0.9)*5)*d;
        }
        ENDCG
        
    }
}