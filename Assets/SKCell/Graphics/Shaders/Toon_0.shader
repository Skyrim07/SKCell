Shader "SKCell/Toon_0"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Steps ("Steps", float) = 3
        _Color ("Color", Color) = (1,0.5,0.5,1)
        _Specular ("Specular Color", Color) = (1,1,1,1)
        _SpecularScale ("Specular Scale", Range(0,1)) = 3
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0,5)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
    
        Pass
        {
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };


            float _OutlineWidth;
            fixed4 _OutlineColor;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.xyz += v.normal * _OutlineWidth * 0.075;
                o.vertex = UnityObjectToClipPos(v.vertex);
        
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    
        Pass
        {
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldNormal : TEXCOORD1;
                float4 worldLight : TEXCOORD2;
                float4 worldView : TEXCOORD3;
                float4 worldPos : TEXCOORD4;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _Specular;

            float _Steps;
            float _SpecularScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        
                o.worldPos =  normalize(mul(unity_ObjectToWorld, v.vertex));
                o.worldNormal = normalize(mul(unity_ObjectToWorld, v.normal));
                o.worldLight = float4(normalize(UnityWorldSpaceLightDir(o.worldPos)),0);
                o.worldView = float4(normalize(UnityWorldSpaceViewDir(o.worldPos)),0);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                float ndl = saturate(dot(i.worldNormal, i.worldLight));

                fixed4 worldHalf = normalize( i.worldLight+ i.worldView);

                fixed spec = dot(i.worldNormal, worldHalf);
				fixed w = fwidth(spec);
                fixed4 specular = _Specular * smoothstep(0, w, spec + (_SpecularScale*0.1) -1);

                float curStep = 0;
                float stepLength = 1.0 / _Steps;
                for(int i = 0; i<_Steps; i++){
                    curStep+=stepLength;
                    if(ndl<curStep){
                        ndl = curStep;
                        break;
                    }
                }

                fixed4 diffuse =  fixed4(1,1,1,1) * ndl * 0.5+0.5;
                
                return col * diffuse + specular;
            }
            ENDCG
        }
    }
}
