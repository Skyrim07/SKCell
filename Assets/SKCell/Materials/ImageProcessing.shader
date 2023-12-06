
Shader "SKCell/ImageProcessing"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_AlphaMask ("AlphaMask", 2D) = "white" {}
		_Color ("Color", color) =(1,1,1,1)
		_AlphaLX("RangeAlphaLX",Range(0,2)) = 0
		_AlphaRX("RangeAlphaRX",Range(-1,1)) = 1
		_AlphaTY("RangeAlphaTY",Range(-1,1)) = 1
		_AlphaBY("RangeAlphaBY",Range(0,2)) = 0
		_AlphaPower("Power",Float) = 0 

		_Hue ("Hue", Range(0,1))=1
		_Brightness ("Brightness", Range(0,5))=1
		_Saturation ("Saturation", Range(0,5))=1
		_Contrast ("Contrast", Range(0,5))=1

		_EdgeAlphaThreshold("Edge Alpha Threshold", Float) = 1.0			
		_EdgeColor("Edge Color", Color) = (0,0,0,1)							
		_EdgeDampRate("Edge Damp Rate", Float) = 2								
		_BaseAlphaThreshold("BaseAlphaThreshold", range(0.1, 1)) = 0.2		
		_ShowOutline ("Show Outline", Int) = 0			

		_SrcBlendMode("SrcBlendMode", Float) = 0
		_DstBlendMode("DstBlendMode", Float) = 0

	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		Blend [_SrcBlendMode] [_DstBlendMode]
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
				fixed4 color  :COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1[9] : TEXCOORD1;
				float4 vertex : SV_POSITION;
				fixed4 color :COLOR0;
			};

			sampler2D _MainTex;
			sampler2D _AlphaMask;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			fixed4 _Color;

			float _AlphaPower;
			sampler2D _AlphaTex;
			float _AlphaLX;
			float _AlphaRX;
			float _AlphaTY;
			float _AlphaBY;

			float _Hue;
			float _Brightness;
			float _Saturation;
			float _Contrast;

			fixed _EdgeAlphaThreshold;
			fixed4 _EdgeColor;
			float _EdgeDampRate;
			float _BaseAlphaThreshold;
			int _ShowOutline;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				color.a = tex2D (_AlphaTex, uv).r;
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}

			fixed Luminance (fixed4 color)
			{
				return dot(color.rgb, fixed3(0.2125, 0.7154, 0.0721));
			}

			float3 hsv2rgb(float3 c){
				float3 rgb = clamp( abs(fmod(c.x*6.0+float3(0.0,4.0,2.0),6)-3.0)-1.0, 0, 1);
				rgb = rgb*rgb*(3.0-2.0*rgb);
				return c.z * lerp( float3(1,1,1), rgb, c.y);
			}
			float3 rgb2hsv(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
				float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

				float d = q.x - min(q.w, q.y);
				float e = 1.0e-10;
				return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			half CalculateAlphaSumAround(v2f i)
			{
				half texAlpha;
				half alphaSum = 0;
				for(int it = 0; it < 9; it ++)
				{
					texAlpha = tex2D(_MainTex, i.uv1[it]).w;
					alphaSum += texAlpha;
				}
 
				return alphaSum;
			}


			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color=v.color;

				half2 uv = v.uv;
 
				o.uv1[0] = uv + _MainTex_TexelSize.xy * half2(-1, -1);
				o.uv1[1] = uv + _MainTex_TexelSize.xy * half2(0, -1);
				o.uv1[2] = uv + _MainTex_TexelSize.xy * half2(1, -1);
				o.uv1[3] = uv + _MainTex_TexelSize.xy * half2(-1, 0);
				o.uv1[4] = uv + _MainTex_TexelSize.xy * half2(0, 0);
				o.uv1[5] = uv + _MainTex_TexelSize.xy * half2(1, 0);
				o.uv1[6] = uv + _MainTex_TexelSize.xy * half2(-1, 1);
				o.uv1[7] = uv + _MainTex_TexelSize.xy * half2(0, 1);
				o.uv1[8] = uv + _MainTex_TexelSize.xy * half2(1, 1);

				return o;
			}


			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = SampleSpriteTexture(i.uv) * i.color ;

				//Hue
				fixed3 hsv = rgb2hsv(col.rgb);
				hsv.x+=_Hue;
				col.rgb=hsv2rgb(hsv);

			

				//Saturation
				fixed luminance = Luminance(col);
				fixed3 luminanceCol = fixed3 (luminance,luminance,luminance);
				col = fixed4(lerp(luminanceCol, col.rgb, _Saturation), col.a);

				//Contrast
				fixed3 grey = fixed3 (0.5,0.5,0.5);
			    col = fixed4(lerp(grey, col.rgb, _Contrast), col.a);

				//Brightness
				col= float4(col.rgb*_Brightness, col.a);

				//Outline
				if(_ShowOutline == 1)
				{
					half alphaSum = CalculateAlphaSumAround(i);
					float isEdgePixel = alphaSum > _EdgeAlphaThreshold ? 1.0 : 0.0;
					float damp = saturate((alphaSum - _EdgeAlphaThreshold) * _EdgeDampRate);
					float isOriginalPixel = col.a > _BaseAlphaThreshold ? 1.0 : 0.0;
					float3 finalColor = lerp(_EdgeColor.rgb, col.rgb, isOriginalPixel);

					col = float4(finalColor.rgb, isEdgePixel * damp);
				}
			    //Alpha Fade
				fixed alphalx = col.a * lerp(1,_AlphaPower,(_AlphaLX-i.uv.x));
				col.a = saturate(lerp(alphalx,col.a,step(_AlphaLX,i.uv.x)));

				fixed alpharx = col.a * lerp(1,_AlphaPower,(i.uv.x-_AlphaRX));
				col.a = saturate(lerp(col.a,alpharx,step(_AlphaRX,i.uv.x)));

				fixed alphaby = col.a * lerp(1,_AlphaPower,(_AlphaBY-i.uv.y));
				col.a = saturate(lerp(alphaby,col.a,step(_AlphaBY,i.uv.y)));

				fixed alphaty = col.a * lerp(1,_AlphaPower,(i.uv.y-_AlphaTY));
				col.a = saturate(lerp(col.a,alphaty,step(_AlphaTY,i.uv.y)));

				//Alpha Mask
				fixed ma = tex2D(_AlphaMask, i.uv).a;
				col.a *= ma;

				float4 resColor= col;



				return resColor;
			}
			ENDCG
		}
	}
}
