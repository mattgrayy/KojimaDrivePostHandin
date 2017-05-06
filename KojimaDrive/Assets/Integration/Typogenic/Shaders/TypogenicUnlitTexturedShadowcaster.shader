Shader "Typogenic/Unlit Textured Shadowcaster Font"
{
	Properties
	{
		_MainTex ("Base (Alpha8)", 2D) = "white" {}
		_FillTex ("Fill Texture (RGBA)", 2D) = "white" {}
		_Smoothness ("Smoothness / Antialiasing (Float)", Float) = 0.85
		_Thickness ("Thickness (Float)", Range(1.0, 0.05)) = 0.5

		// Texture hue shift 
		// MIT License, Copyright 2015, Gregg Tavares. All rights reserved.
		//_HSVRangeMin("Texture HSV Affect Range Min", Range(0, 1)) = 0
		//_HSVRangeMax("Texture HSV Affect Range Max", Range(0, 1)) = 1
		
		_HSVAAdjust("Texture HSVA Adjust", Vector) = (0, 0, 0, 0)

		_SaturateAmount("Saturate Amount", Range(-2, 2)) = 0
		_BrightnessAmount("Brightness Amount", Range(-2, 2)) = 0
		_ContrastAmount("Contrast Amount", Range(-3, 3)) = 0 

		// OUTLINED
		_OutlineColor ("Outline Color (RGBA)", Color) = (0, 0, 0, 1)
		_OutlineThickness ("Outline Thickness (Float)", Range(1.0, 0.1)) = 0.25

		// SECOND OUTLINE (added for Kojima Drive)
		_OutlineColor2("Outline Color (RGBA)", Color) = (0, 0, 0, 1)
		_OutlineThickness2("Outline Thickness (Float)", Range(1.0, 0.1)) = 0.25

		// GLOW
		_GlowColor ("Glow Color (RGBA)", Color) = (0, 0, 0, 1)
		_GlowStart ("Glow Start", Range(0.0, 1.0)) = 0.1
		_GlowEnd ("Glow End", Range(0.0, 1.0)) = 0.9

		// GLOBAL_MULTIPLIER
		_GlobalMultiplierColor ("Global Color Multiplier (RGBA)", Color) = (1, 1, 1, 1)

		// SHADOW CUTOFF
		_ShadowCutoff ("Shadow Cutoff",Range(0.01, 1.0)) = .7
	}
	SubShader
	{
		Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
            Offset 1, 1
           
            Fog {Mode Off}
            ZWrite On ZTest LEqual Cull Off
   
            CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag
	        #pragma multi_compile OUTLINED_ON OUTLINED_OFF
			#pragma multi_compile OUTLINED_ON2 OUTLINED_OFF2
	        #pragma multi_compile_shadowcaster
	        #pragma fragmentoption ARB_precision_hint_fastest
	        #include "UnityCG.cginc"
	       
	        float4 _MainTex_ST;
	        sampler2D _MainTex;
	        float4 _FillTex_ST;
			sampler2D _FillTex;
			half _OutlineThickness;
			half _OutlineThickness2;
	        half _ShadowCutoff;
			half _Smoothness;
			half _Thickness;

     		struct appdata {
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			    float4 texcoord1 : TEXCOORD1;
			}; 

	        struct v2f {
	            V2F_SHADOW_CASTER;
	            float2  uv  : TEXCOORD1;
	            float2  uv2 : TEXCOORD2;
	        };
	       
	        v2f vert( appdata v ) {
	            v2f o;
	            TRANSFER_SHADOW_CASTER(o)
	            o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
	            o.uv2 = TRANSFORM_TEX(v.texcoord1, _FillTex);
	            return o;
	        }
	       
	        float4 frag(v2f i) : COLOR {
	            half dist = tex2D(_MainTex, i.uv).a;
				half smoothing = fwidth(dist) * _Smoothness;
				half alpha = smoothstep(_Thickness - smoothing, _Thickness + smoothing, dist);
				half4 color = tex2D(_FillTex, i.uv2);
				
				alpha *= color.a;

#if OUTLINED_ON && !OUTLINED_ON2
				half outlineAlpha = smoothstep(_OutlineThickness - smoothing, _OutlineThickness + smoothing, dist);

				if (outlineAlpha > _ShadowCutoff)
				{
					alpha = outlineAlpha;
				}
#endif

#if OUTLINED_ON2 && !OUTLINED_ON
				half outlineAlpha2 = smoothstep(_OutlineThickness2 - smoothing, _OutlineThickness2 + smoothing, dist);

				if (outlineAlpha2 > _ShadowCutoff)
				{
					alpha = outlineAlpha2;
				}
#endif

#if OUTLINED_ON2 && OUTLINED_ON
				half outlineAlpha2 = smoothstep(_OutlineThickness2 - smoothing, _OutlineThickness2 + smoothing, dist);
				half outlineAlpha = smoothstep(_OutlineThickness - smoothing, _OutlineThickness + smoothing, dist);

				if (outlineAlpha > outlineAlpha2) {
					if (outlineAlpha > _ShadowCutoff)
					{
						alpha = outlineAlpha;
					}
				}
				else {
					if (outlineAlpha2 > _ShadowCutoff)
					{
						alpha = outlineAlpha2;
					}
				}
#endif
				
				clip(alpha - _ShadowCutoff);
	           
	            SHADOW_CASTER_FRAGMENT(i)
	        }
            ENDCG
       	}
       	
       	
       	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200
		ZWrite Off

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma glsl
			#pragma target 3.0
			#pragma multi_compile HUESHIFT_ON HUESHIFT_OFF
			#pragma multi_compile OUTLINED_ON OUTLINED_OFF
			#pragma multi_compile OUTLINED_ON2 OUTLINED_OFF2
			#pragma multi_compile GLOW_ON GLOW_OFF
			#pragma multi_compile GLOBAL_MULTIPLIER_ON GLOBAL_MULTIPLIER_OFF
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _FillTex;
			half4 _FillTex_ST;
			half _Smoothness;
			half _Thickness;

			// OUTLINED
			half4 _OutlineColor;
			half _OutlineThickness;

			half4 _OutlineColor2;
			half _OutlineThickness2;

			// HUE SHIFT
			//float _HSVRangeMin = 0.0f;
			//float _HSVRangeMax = 1.0f;
			float4 _HSVAAdjust;

			float _SaturateAmount;
			float _BrightnessAmount;
			float _ContrastAmount;

			// GLOW
			half4 _GlowColor;
			half _GlowStart;
			half _GlowEnd;
			
			// GLOBAL_MULTIPLIER
			half4 _GlobalMultiplierColor;

			struct vertexInput
			{
				half4 vertex : POSITION;
				half2 texcoord0 : TEXCOORD0;
				half2 texcoord1 : TEXCOORD1;
			};

			struct fragmentInput
			{
				half4 position : SV_POSITION;
				half2 texcoord0 : TEXCOORD0;
				half2 texcoord1 : TEXCOORD1;
			};

			fragmentInput vert(vertexInput i)
			{
				fragmentInput o;
				o.position = mul(UNITY_MATRIX_MVP, i.vertex);
				o.texcoord0 = i.texcoord0;
				o.texcoord1 = TRANSFORM_TEX(i.texcoord1, _FillTex);
				return o;
			}

			float4 desat(float4 col)
			{
				float4 c = col;
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

			half4 frag(fragmentInput i) : COLOR
			{
				half dist = tex2D(_MainTex, i.texcoord0).a;
				half4 color = tex2D(_FillTex, i.texcoord1);

#if HUESHIFT_ON
				float3 hsv = rgb2hsv(color.rgb);
				float affectMult = step(0, hsv.r) * step(hsv.r, 1); // float affectMult = step(_HSVRangeMin, hsv.r) * step(hsv.r, _HSVRangeMax);
				float3 rgb = hsv2rgb(hsv + _HSVAAdjust.xyz * affectMult);
				color = half4(rgb.x, rgb.y, rgb.z, color.a + _HSVAAdjust.a);
#endif
				color = desat(color);
				color = darken(color);
				color = contrast(color);
				half smoothing = fwidth(dist) * _Smoothness;
				half alpha = smoothstep(_Thickness - smoothing, _Thickness + smoothing, dist);
				half4 finalColor = half4(color.rgb, color.a * alpha);

				// OUTLINED
#if OUTLINED_ON && !OUTLINED_ON2

				half outlineAlpha = smoothstep(_OutlineThickness - smoothing, _OutlineThickness + smoothing, dist);
				half4 outline = half4(_OutlineColor.rgb, _OutlineColor.a * outlineAlpha);
				finalColor = lerp(outline, finalColor, alpha);

#endif

				// OUTLINED 2
#if OUTLINED_ON2 && !OUTLINED_ON

				half outlineAlpha2 = smoothstep(_OutlineThickness2 - smoothing, _OutlineThickness2 + smoothing, dist);
				half4 outline2 = half4(_OutlineColor2.rgb, _OutlineColor2.a * outlineAlpha2);
				finalColor = lerp(outline2, finalColor, alpha);

#endif

				// OUTLINED BOTH
#if OUTLINED_ON2 && OUTLINED_ON

				// Calc outline 1
				half outlineAlpha = smoothstep(_OutlineThickness - smoothing, _OutlineThickness + smoothing, dist);
				half4 outline = half4(_OutlineColor.rgb, _OutlineColor.a * outlineAlpha);

				// Calc outline 2
				half outlineAlpha2 = smoothstep(_OutlineThickness2 - smoothing, _OutlineThickness2 + smoothing, dist);
				half4 outline2 = half4(_OutlineColor2.rgb, _OutlineColor2.a * outlineAlpha2);

				// Combine outlines
				half4 finaloutline = outline2;
				finaloutline = lerp(finaloutline, outline, outlineAlpha);

				// Calc final colour
				finalColor = lerp(finaloutline, finalColor, alpha);

#endif

				// GLOW
				#if GLOW_ON

				half glowAlpha = smoothstep(_GlowStart, _GlowEnd, dist);
				finalColor = lerp(half4(_GlowColor.rgb, _GlowColor.a * glowAlpha), finalColor, finalColor.a);

				#endif
				
				// GLOBAL_MULTIPLIER
				#if GLOBAL_MULTIPLIER_ON

				return finalColor * _GlobalMultiplierColor;

				#endif

				#if GLOBAL_MULTIPLIER_OFF

				return finalColor;

				#endif
			}

			ENDCG
		}
	}

	FallBack off
	CustomEditor "TypogenicMaterialEditor"
}
