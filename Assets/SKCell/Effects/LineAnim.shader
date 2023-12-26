Shader "SKCell/Curve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _TipColor("Tip Color", Color) = (1,1,1,1)
        _Speed("Speed", float) = 1
        _Phase("Phase", float) = 1
        _Thickness("Thickness", float) = 0.5
        _Amplitude("Amplitude", float) = 0.2
        _Frequency("Frequency", float) = 15
        _Length("Length", float) = 1
        _FixPoint("Fix Points", int) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        ZWrite Off
        Cull Off
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Speed, _Amplitude, _Frequency, _Thickness, _Phase, _Length;

            fixed4 _BaseColor, _TipColor, _Color;
            int _FixPoint;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float ouvy = i.uv.y;

                i.uv.y += sin(_Frequency *i.uv.x + _Time.y * _Speed+ _Phase) * _Amplitude;
                i.uv.y -= 0.5;
                i.uv.y /= _Thickness;
                i.uv.y += 0.5;
                if(_FixPoint == 1)
                    i.uv.y = lerp(ouvy, i.uv.y, i.uv.x);
                if(_FixPoint == 2){
                    i.uv.y = lerp(ouvy, i.uv.y, i.uv.x);
                    i.uv.y = lerp(ouvy, i.uv.y, 1-i.uv.x);
                }

                fixed4 col = tex2D(_MainTex, i.uv);
                col *=lerp( _BaseColor, _TipColor, i.uv.x);

                col.a *= smoothstep(_Length, _Length - 0.1, i.uv.x);

                col *= saturate(_Color);
                return col;
            }
            ENDCG
        }
    }
}
