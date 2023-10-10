Shader "AlexLiu/SampleBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
         _Color ("Color", Color) = (1,1,1,1)
        _Blur("Blur", Range(0,1)) = 0.5
        _SampleCount("Sample Count", Range(4,32)) = 16
    }
    SubShader
    {

        Tags{"RenderType" = "Transparent" "Queue" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
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
            float _Blur;
            float _SampleCount;
            fixed4 _Color;

            float rand11(float x) {
                return frac(sin(x * 789.95) * 123.36);
            }
            float rand21(float2 xy) {
                return frac(sin(xy.x * 789.95) * 123.36*xy.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = fixed4(0,0,0,0);
                
                float a = rand21(i.uv) * 6.28;
                float4 scol = float4(0, 0, 0, 0);

                float blur = _Blur*.2;

                for (int j = 0; j < _SampleCount; j++) {
                    float2 offset = float2(sin(a), cos(a)) * rand11(a)*blur;
                    scol += tex2D(_MainTex, i.uv + offset);
                    a++;
                }
                col = scol / _SampleCount;

                col *= _Color;
                col.a=.00;
                return col;
            }
            ENDCG
        }
    }
}
