Shader "Custom/Monster"
{
    Properties {
        _MainTex ("Main Texture", 2D) = "white" {}
        _DistortionStrength ("Distortion Strength", Range(0.0, 1.0)) = 0.2
        _ScrollSpeed ("Scroll Speed", Range(0.0, 1.0)) = 0.5
    }
 
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        CGPROGRAM
        #pragma surface surf Lambert alpha
 
        sampler2D _MainTex;
        float _DistortionStrength;
        float _ScrollSpeed;
 
        struct Input {
            float2 uv_MainTex;
        };
 
        void surf (Input IN, inout SurfaceOutput o) {
            // Apply distortion effect
            float2 distortion = tex2D(_MainTex, IN.uv_MainTex).rg * _DistortionStrength;
            IN.uv_MainTex += distortion;
 
            // Scroll the texture
            float scrollOffset = _ScrollSpeed * _Time.y;
            IN.uv_MainTex.x += scrollOffset;
 
            // Sample the texture and consider transparency
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb * c.a;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
