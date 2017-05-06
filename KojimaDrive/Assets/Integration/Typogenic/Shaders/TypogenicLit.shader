Shader "Typogenic/Lit Font"
{
	Properties
	{
		_MainTex ("Base (Alpha8)", 2D) = "white" {}
		_Smoothness ("Smoothness / Antialiasing (Float)", Float) = 0.85
		_Thickness ("Thickness (Float)", Range(1.0, 0.05)) = 0.5

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
	}
	SubShader
	{
		
		
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		
				
		ZWrite Off

		CGPROGRAM
		#pragma surface surf Lambert alpha nolightmap nodirlightmap
		#pragma glsl
		#pragma target 3.0
		#pragma multi_compile OUTLINED_ON OUTLINED_OFF
		#pragma multi_compile OUTLINED_ON2 OUTLINED_OFF2
		#pragma multi_compile GLOW_ON GLOW_OFF
		#pragma multi_compile GLOBAL_MULTIPLIER_ON GLOBAL_MULTIPLIER_OFF
		
		
		sampler2D _MainTex;
		half _Smoothness;
		half _Thickness;

		// OUTLINED
		half4 _OutlineColor;
		half _OutlineThickness;

		half4 _OutlineColor2;
		half _OutlineThickness2;
		
		// GLOW
		half4 _GlowColor;
		half _GlowStart;
		half _GlowEnd;
			
		// GLOBAL_MULTIPLIER
		half4 _GlobalMultiplierColor;
		half4 _Color;

		struct Input
		{
			half2 uv_MainTex;
			half4 color : Color;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			half dist = tex2D(_MainTex, IN.uv_MainTex).a;

			half smoothing = fwidth(dist) * _Smoothness;
			half alpha = smoothstep(_Thickness - smoothing, _Thickness + smoothing, dist);

			half4 finalColor = half4(IN.color.rgb, IN.color.a * alpha);

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
				
			o.Albedo = finalColor.rgb * _GlobalMultiplierColor.rgb;
			o.Alpha = finalColor.a * _GlobalMultiplierColor.a;

			#endif

			#if GLOBAL_MULTIPLIER_OFF
				
			o.Albedo = finalColor.rgb;
			o.Alpha = finalColor.a;

			#endif
			
		}
		ENDCG
	} 

			
	FallBack off
	CustomEditor "TypogenicMaterialEditor"
}
