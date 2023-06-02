Shader "Custom/MotionBlur" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Speed", Range(0, 10)) = 1
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
            float _Speed;
            
            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target {
                float2 blurUV = i.uv - float2(0.5, 0.5);
                float speed = _Speed * 0.1;
                
                float2 sampleUV1 = blurUV - speed;
                float2 sampleUV2 = blurUV;
                float2 sampleUV3 = blurUV + speed;
                
                fixed4 color1 = tex2D(_MainTex, sampleUV1);
                fixed4 color2 = tex2D(_MainTex, sampleUV2);
                fixed4 color3 = tex2D(_MainTex, sampleUV3);
                
                fixed4 finalColor = (color1 + color2 + color3) / 3;
                
                return finalColor;
            }
            
            ENDCG
        }
    }
}
