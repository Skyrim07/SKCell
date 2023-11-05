Shader "SKCell/Dissolve_0"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1,1,1,1)
        _HighlightColor ("Highlight Color", Color) = (0.9,0.8,0.7,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        _Threshold ("Threshold", range(0,1)) = 0.5        
        _HighlightThreshold ("Highlight Threshold", float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
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
                float2 noiseuv : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _Noise;
            float4 _MainTex_ST;
            float4 _Noise_ST;
            fixed4 _Color;
            fixed4 _HighlightColor;

            float _Threshold;
            float _HighlightThreshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.noiseuv = TRANSFORM_TEX(v.uv, _Noise);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
	            fixed4 noiseTex = tex2D(_Noise,i.noiseuv);
                fixed noiseDiff = pow(saturate(noiseTex.r- _Threshold)/ _HighlightThreshold, 5);
	            clip(noiseTex.r- _Threshold);
                col = lerp(col, _HighlightColor,  saturate(1 - noiseDiff)) ;          
                return col;
            }
            ENDCG
        }
    }
}
