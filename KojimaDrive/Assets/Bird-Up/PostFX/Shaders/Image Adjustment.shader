Shader "Post Processing/Image Adjustment"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SaturateAmount("Saturate Amount", Range(-2, 2)) = 0
		_BrightnessAmount("Brightness Amount", Range(-2, 2)) = 0
		_ContrastAmount("Contrast Amount", Range(-3, 3)) = 0
		_HSVRangeMin("HSV Affect Range Min", Range(0, 1)) = 0
		_HSVRangeMax("HSV Affect Range Max", Range(0, 1)) = 1
		_HSVAAdjust("HSVA Adjust", Vector) = (0, 0, 0, 0)
	}
	SubShader
	{
		// No culling or depth
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
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _SaturateAmount;
			float _BrightnessAmount;
			float _ContrastAmount;
			float _HSVRangeMin;
			float _HSVRangeMax;
			float4 _HSVAAdjust;

			float3 rgb2hsv(float3 c) {
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
				float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

				float d = q.x - min(q.w, q.y);
				float e = 1.0e-10;
				return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			float3 hsv2rgb(float3 c) {
				c = float3(c.x, clamp(c.yz, 0.0, 1.0));
				float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
				return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
			}

			float4 hsv(float4 col)
			{
				float4 o = float4(1, 0, 0, 0.2);
				float3 hsv = rgb2hsv(col.rgb);
				float affectMult = step(_HSVRangeMin, hsv.r) * step(hsv.r, _HSVRangeMax);
				float3 rgb = hsv2rgb(hsv + _HSVAAdjust.xyz * affectMult);
				return float4(rgb, col.a + _HSVAAdjust.a);
			}

			float4 desat(sampler2D tex, float2 uv)
			{
				float4 c = tex2D(tex, uv);
				float d = (c.r + c.g + c.b) / 3.0f;
				float4 cd = float4(d, d, d, c.a);
				c = lerp(cd, c, _SaturateAmount + 1);

				return c;
			}

			float4 darken(float4 col) {
				float4 w_tex = col;
				w_tex.rgb = lerp(float3(0, 0, 0), col.rgb, _BrightnessAmount + 1);
				return w_tex;
			}

			float4 contrast(float4 col) {
				float4 w_tex = col;
				w_tex.rgb = ((col.rgb - 0.5) * (_ContrastAmount + 1)) + 0.5;
				return w_tex;
			}

			float4 frag (v2f i) : SV_Target
			{
				float4 col = desat(_MainTex, i.uv);
				col = darken(col);
				col = contrast(col);
				col = hsv(col);
				return col;
			}
			ENDCG
		}
	}
}
