Shader "SKCell/Dither"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha ("Alpha", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float4x4 thresholdMatrix =
                {
                        1.0f / 17.0f, 9.0f / 17.0f, 3.0f / 17.0f, 11.0f / 17.0f,
                        13.0f / 17.0f, 5.0f / 17.0f, 15.0f / 17.0f, 7.0f / 17.0f,
                        4.0f / 12.0f, 1.0f / 17.0f, 2.0f / 17.0f, 10.0f / 17.0f,
                        16.0f / 17.0f, 8.0f / 17.0f, 14.0f / 17.0f, 6.0f / 17.0f,
                };

                float4x4 rowAccess = { 1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1 };

                float2 pos = i.screenPos.xy / i.screenPos.w;
                pos *= _ScreenParams.xy;

                clip(_Alpha - thresholdMatrix[fmod(pos.x, 4)] * rowAccess[fmod(pos.x, 4)]);

                return col;
            }
            ENDCG
        }
    }
}
