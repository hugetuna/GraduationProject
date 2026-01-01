Shader "Unlit/GauseBlur"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (1, 1, 1, 0.5)
        _BlurSize ("Blur Size", Range(0, 10)) = 1
        
        // UI 遮罩屬性保留...
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

        // 1. 使用一個非常獨特的名稱，避免與 Unity 內建名稱衝突
        GrabPass { "_DialogGrabTexture" } 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            // 2. 在這裡宣告，但絕對不要寫在頂部的 Properties 裡
            sampler2D _DialogGrabTexture; 
            float4 _DialogGrabTexture_TexelSize;
            float _BlurSize;
            fixed4 _TintColor;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 3. 安全檢查：如果沒抓到貼圖（例如在 Editor 預覽時），直接回傳 TintColor
                if (i.screenPos.w <= 0) return _TintColor;

                float2 uv = i.screenPos.xy / i.screenPos.w;
                float step = _DialogGrabTexture_TexelSize.x * _BlurSize;

                // 採樣邏輯...
                fixed4 col = tex2D(_DialogGrabTexture, uv) * 0.4;
                col += tex2D(_DialogGrabTexture, uv + float2(step, 0)) * 0.3;
                col += tex2D(_DialogGrabTexture, uv - float2(step, 0)) * 0.3;
                
                return col * _TintColor;
            }
            ENDCG
        }
    }
}
