Shader "Custom/Vignette" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _VignetteAmount ("Vignette Amount", Range(0, 1)) = 0.5
    }
    
    SubShader {
        Cull Off ZWrite Off ZTest Always
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float _VignetteAmount;
            
            v2f vert(const appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target {
                float vignetteFactor = length(i.uv - 0.5) / 0.5;
                vignetteFactor = saturate(1.0 - vignetteFactor * _VignetteAmount);
                
                const fixed4 texColor = tex2D(_MainTex, i.uv);
                const fixed4 vignetteColor = texColor * vignetteFactor;
                
                return vignetteColor;
            }
            
            ENDCG
        }
    }
}
