Shader "SKCell/Grass_2"
{
	Properties
	{
		_TintTex("Tint Texture", 2D) = "white" {}
		_TintTex2("Tint Texture2", 2D) = "white" {}
		_TopColor("Top Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (0,0,0,1)
		_TintColor("Tint Color", Color) = (0,0,0,1)
		_TintColor2("Tint Color2", Color) = (0,0,0,1)
		_Tint("Tint", Range(0,1)) = 0.3
		_Tint2("Tint2", Range(0,1)) = 0.3
		_Bend("Bend", Range(0,1)) = 0.2
		_BladeWidth("Blade Width", float) = 0.05
		_BladeWidthRandom("Blade Width Random", float) = 0.02
		_BladeHeight("Blade Height", float) = 0.5
		_BladeHeightRandom("Blade Height Random", float) = 0.3
		_BladeCurve("Blade Curve", float) = 0.5
		_BladeForward("Blade Forward", float) = 0.5
		_BladeForwardRandom("Blade Forward Random", float) = 0.3

		_WindDistortionMap("Wind Distortion Map", 2D) = "white" {}
		_WindFrequency("Wind Frequency", vector) = (0.05, 0.05, 0, 0)
		_WindStrength("Wind Strength", float) = 1
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100
			Cull Off

				CGINCLUDE
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"
				#define BLADE_SEGMENTS 3
				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float4 tangent :TANGENT;
					float3 normal :NORMAL;
				};

				struct v2g
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					float4 tangent : TEXCOORD1;
					float3 normal :TEXCOORD2;
					float3 worldPos : TEXCOORD3;
				};

				struct g2f
				{
					float2 uv : TEXCOORD0;
					float2 vuv : TEXCOORD1;
					float4 vertex : SV_POSITION;
					float3 worldPos : TEXCOORD2;
					SHADOW_COORDS(3)
				};

				sampler2D _TintTex;
				sampler2D _TintTex2;
				float4 _TintTex_ST;
				sampler2D _WindDistortionMap;
				float4 _WindDistortionMap_ST;
				float2 _WindFrequency;
				float _WindStrength;

				fixed4 _TopColor, _BottomColor, _TintColor, _TintColor2;
				float _Tint, _Tint2;
				float _Bend;
				float _BladeCurve;
				float _BladeHeight;
				float _BladeHeightRandom;
				float _BladeWidth;
				float _BladeForward;
				float _BladeWidthRandom;
				float _BladeForwardRandom;

				float3 _GrassInteractCenter;
				float _GrassInteractRadius;
				float _InteractiveStrength;


				float N31(float3 p)
				{
					return frac(sin(dot(p, float3(1928.12, 3846.09, 801263.3))) * 1299.1241 + 918.2);
				}

				float N11(float a)
				{
					return frac(sin(a * 34525.14) * 32.615);
				}

				float3x3 GetRotationMatrix(float angle, float3 axis)
				{
					float c, s;
					sincos(angle, s, c);

					float t = 1 - c;
					float x = axis.x;
					float y = axis.y;
					float z = axis.z;

					return float3x3(
						t * x * x + c, t * x * y - s * z, t * x * z + s * y,
							t * x * y + s * z, t * y * y + c, t * y * z - s * x,
							t * x * z - s * y, t * y * z + s * x, t * z * z + c
					);
				}

				v2g vert(appdata v)
				{
					v2g o;
					o.vertex = v.vertex;
					o.uv = TRANSFORM_TEX(v.uv, _TintTex);
					o.tangent = v.tangent;
					o.normal = v.normal;
					o.worldPos = mul(unity_ObjectToWorld, v.vertex);
					return o;
				}

				g2f GetG2F(float2 uv, float3 pos, float3 worldPos, float2 vuv)
				{
					g2f o;
					o.uv = uv;
					o.vuv = vuv;
					o.worldPos = worldPos;
					o.vertex = UnityObjectToClipPos(float4(pos.xyz, 1));
					TRANSFER_SHADOW(o);
					return o;
				}

				g2f GetGrass(float2 uv, float2 vuv, float3 vertexPosition, float3 worldPos, float width, float height, float forward, float3x3 transformMatrix)
				{
					float3 tangentPoint = float3(width, forward, height);

					float3 localPosition = vertexPosition + mul(transformMatrix, tangentPoint);
					return GetG2F(uv, localPosition, worldPos, vuv);
				}

				[maxvertexcount(BLADE_SEGMENTS * 2 + 1)]
				void geom(triangle v2g IN[3] : SV_POSITION, inout TriangleStream<g2f> triStream)
				{
					g2f o;

					float3 pos = IN[0].vertex;
					float3 worldPos = IN[0].worldPos;
					float3 vnormal = IN[0].normal;
					float4 vtangent = IN[0].tangent;
					float2 vuv = IN[1].uv;
					float3 vbinormal = cross(vnormal, vtangent) * vtangent.w;
					float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y * 0.05;

					float3x3 tbn = {
						vtangent.x, vbinormal.x, vnormal.x,
						vtangent.y, vbinormal.y, vnormal.y,
						vtangent.z, vbinormal.z, vnormal.z,
					};

					float3x3 rotationMatrix = GetRotationMatrix(N31(pos) * UNITY_TWO_PI, float3(0, 0, 1));
					float3x3 bendMatrix = GetRotationMatrix(N31(pos.zzx) * UNITY_PI * 0.5 * _Bend, float3(-1, 0, 0));

					float2 windSample = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * _WindStrength;
					float3 wind = normalize(float3(windSample.x, windSample.y, 0));
					float3x3 windMatrix = GetRotationMatrix(UNITY_PI * windSample, wind);

					//interaction
					float3 dis = distance(_GrassInteractCenter, worldPos); // distance for radius
					float3 radius = 1 - saturate(dis / _GrassInteractRadius); // in world radius based on objects interaction radius
					float3 sphereDisp = worldPos - _GrassInteractCenter; // position comparison
					sphereDisp *= radius; // position multiplied by radius for falloff
					sphereDisp = clamp(sphereDisp.xyz * _InteractiveStrength, -0.8, 0.8);
					sphereDisp *= (N11(worldPos) / 5.0f + 0.8f);

					float3x3 fixTransformMatrix = mul(tbn, rotationMatrix);
					float3x3 transformMatrix = mul(mul(fixTransformMatrix, bendMatrix), windMatrix);

					float height = (N31(pos.zyx) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
					float width = (N31(pos.zxy) * 2 - 1) * _BladeWidthRandom + _BladeWidth;
					float forward = (N31(pos.yyz) * 2 - 1) * _BladeForwardRandom + _BladeForward;

					for (int i = 0; i < BLADE_SEGMENTS; i++)
					{
						float t = i / (float)BLADE_SEGMENTS;
						float segmentHeight = height * t;
						float segmentWidth = width * (1 - t);
						float segmentForward = pow(t,_BladeCurve) * forward;
						float3x3 segmentMatrix = i == 0 ? fixTransformMatrix : transformMatrix;

						float3 newPos = i == 0 ? pos : pos + ((float3(sphereDisp.x, sphereDisp.y, sphereDisp.z)) * t);

						triStream.Append(GetGrass(float2(0, t), vuv, newPos, worldPos, segmentWidth, segmentHeight, segmentForward, segmentMatrix));
						triStream.Append(GetGrass(float2(1, t), vuv, newPos, worldPos ,-segmentWidth, segmentHeight, segmentForward, segmentMatrix));
					}

					triStream.Append(GetGrass(float2(0.5, 1), vuv, pos + float3(sphereDisp.x * 1.3, sphereDisp.y, sphereDisp.z * 1.3), worldPos, 0, height, forward, transformMatrix));
					triStream.RestartStrip();
				}

				ENDCG

					Pass
				{
					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma geometry geom

					fixed4 frag(g2f i) : SV_Target
					{
					fixed4 col = lerp(_BottomColor, _TopColor, i.uv.y);
					col = lerp(col, _TintColor, tex2D(_TintTex, i.vuv) * _Tint);
					col = lerp(col, _TintColor2, tex2D(_TintTex2, i.vuv) * _Tint2);
					SHADOW_ATTENUATION(i);
					return col;
				}
				ENDCG
			}

			Pass
			{
				Tags
				{
					"LightMode" = "ForwardAdd"
				}
				Blend OneMinusDstColor One
				ZWrite Off

				CGPROGRAM
				#pragma vertex vert
				#pragma geometry geom
				#pragma fragment frag                                   
				#pragma multi_compile_fwdadd_fullforwardshadows

				float4 frag(g2f i) : SV_Target
				{
					UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
					float3 pointlights = atten * _LightColor0.rgb;
					return float4(pointlights, 1);
				}
				ENDCG
				}

		}
}
