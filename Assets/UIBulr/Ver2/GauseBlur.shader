Shader "UI/SimpleBlur"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 1.0
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 簡單的 4 點採樣模糊
                float2 res = _MainTex_TexelSize.xy * _BlurSize;
                fixed4 col = tex2D(_MainTex, i.uv);
                col += tex2D(_MainTex, i.uv + float2(res.x, res.y));
                col += tex2D(_MainTex, i.uv + float2(-res.x, res.y));
                col += tex2D(_MainTex, i.uv + float2(res.x, -res.y));
                col += tex2D(_MainTex, i.uv + float2(-res.x, -res.y));
                return col / 5.0;
            }
            ENDCG
        }
    }
}
