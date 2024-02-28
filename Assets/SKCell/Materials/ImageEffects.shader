Shader "SKCell/ImageEffects"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Texture", 2D) = "white" {}
        _Color("Color", Color)=(1,1,1,1)
        _Value("Value", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
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
                float2 nuv : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            fixed _Value;
            fixed4 _Color;

            int _InverseColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.nuv = TRANSFORM_TEX(v.uv, _NoiseTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv)*_Color;
                fixed4 ncol = tex2D(_NoiseTex, i.nuv);

                //centered uv
                float2 cuv = i.uv * 2 - 1;

                //dissolve
                float ds = _Value * 1.4 - 0.2;
                ds =lerp(ds, ds* (1 - max(abs(cuv.x),abs(cuv.y))), 1-saturate(_Value));
                col.a *= saturate(smoothstep(ncol-.2, ncol+.2, ds-length(cuv)));
 
                float r = 0.5 * (_Value);
                col.rgb+=smoothstep(r-.1,r+.1,ncol.r)  *.2;

                col.a = _InverseColor==1? 1-col.a:col.a;
                return col;
            }
            ENDCG
        }
    }
}
