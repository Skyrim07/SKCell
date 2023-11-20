Shader "SKCell/WorldGridXY"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _Color2 ("Color 2", Color) = (0,0,0,1)
        _LineWidth ("_LineWidth", Range(0,1)) = 0.5
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
                float4 worldPos : TEXCOORD1;
            };

            fixed4 _Color1, _Color2;
            fixed _LineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float diffX = abs(frac(i.worldPos.x) - 0.5);
                diffX = step(0.3*_LineWidth, diffX);
                float diffZ = abs(frac(i.worldPos.y) - 0.5);
                diffZ = step(0.3 * _LineWidth, diffZ);

                fixed diff = diffX * diffZ;

                return lerp(_Color2, _Color1, diff);
            }
            ENDCG
        }
    }
}
