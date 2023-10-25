Shader "SKCell/BlockSplit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaxRGBSplitX ("SplitX", float ) =1
        _MaxRGBSplitY ("SplitY", float ) =1
        _Speed ("Speed", float ) =1
        _BlockSize ("BlockSize", float ) =1
    }
    SubShader
    {
        Cull Off

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
                float2 texcoord : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.texcoord = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _MaxRGBSplitX;
            float _MaxRGBSplitY;
            float _Speed;
            float _BlockSize;

            inline float randomNoise(float2 seed)
            {
                return frac(sin(dot(seed * floor(_Time.y * _Speed), float2(17.13, 3.71))) * 43758.5453123);
            }

            inline float randomNoise(float seed)
            {
                return randomNoise(float2(seed, 1.0));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half2 block = randomNoise(floor(i.texcoord * _BlockSize));

                float displaceNoise = pow(block.x, 8.0) * pow(block.x, 3.0);
                float splitRGBNoise = pow(randomNoise(7.2341), 17.0);
                float offsetX = displaceNoise - splitRGBNoise * _MaxRGBSplitX;
                float offsetY = displaceNoise - splitRGBNoise * _MaxRGBSplitY;

                float noiseX = 0.05 * randomNoise(13.0);
                float noiseY = 0.05 * randomNoise(7.0);
                float2 offset = float2(offsetX * noiseX, offsetY* noiseY);

                half4 colorR = tex2D(_MainTex, i.texcoord);
                half4 colorG = tex2D(_MainTex, i.texcoord + offset);
                half4 colorB = tex2D(_MainTex, i.texcoord - offset);

                return fixed4(colorR.r , colorG.g, colorB.z, (colorR.a + colorG.a + colorB.a));
            }
            ENDCG
        }
    }
}
