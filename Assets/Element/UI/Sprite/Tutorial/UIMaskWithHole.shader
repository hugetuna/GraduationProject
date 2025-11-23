Shader "UI/MaskWithHole"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,0.75)
        _HoleCenter ("Hole Center", Vector) = (0.5, 0.5, 0, 0)
        _HoleSize ("Hole Size", Vector) = (0.2, 0.2, 0, 0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float4 _HoleCenter;
            float4 _HoleSize;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 diff = abs(i.uv - _HoleCenter.xy);
                if (diff.x < _HoleSize.x && diff.y < _HoleSize.y)
                {
                    return float4(0,0,0,0); // 挖洞：透明
                }
                return _Color;
            }
            ENDCG
        }
    }
}
