Shader "SKCell/SKPostProcessMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SatTex ("Sat Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _SatTex;

            float4 pos_sat[16];
            float radius_sat[16];
            float blur_sat[16];
            float value_sat[16];
            float shape_sat[16];

            float circle(float2 uv, float2 pos, float radius, float blur) {
                return smoothstep(radius, radius - blur, length(uv - pos));
            }

            float rect(float2 uv, float2 pos, float radius, float blur)
            {
                float band1 = smoothstep(pos.x - radius-blur, pos.x - radius+blur, uv.x);
                float band2 = smoothstep(pos.x + radius+blur, pos.x + radius-blur, uv.x);
                float band3 = smoothstep(pos.y + radius+blur, pos.y + radius-blur, uv.y);
                float band4 = smoothstep(pos.y - radius-blur, pos.y - radius+blur, uv.y);
                return band1* band2* band3* band4;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed l = Luminance(col);
                fixed4 lcol = fixed4(l, l, l, 1);
                
                float2 uv = float2((i.uv.x-0.5) * (_ScreenParams.x / _ScreenParams.y), i.uv.y-0.5);
                uv += 0.5;
                
                float c = 0;
                for (int k = 0; k < 16; k++)
                {
                    if(shape_sat[k]==0)
                        c += circle(uv, pos_sat[k].xy, radius_sat[k], blur_sat[k] * 0.2) * value_sat[k];
                    if (shape_sat[k] == 1)
                        c += rect(uv, pos_sat[k].xy, radius_sat[k] , blur_sat[k] * 0.2+0.0001) * value_sat[k];
                }
                col = lerp(col, lcol, saturate(c));
                return col;
            }
            ENDCG
        }
    }
}
