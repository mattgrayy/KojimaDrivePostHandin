Shader "Bam/SkidMarkShader" {
	Properties
	{
		_Color("Color", Color) = (0,0,0,1)
		_Alpha("Alpha Mult", Range(0,1)) = 1.0
		_MainTex("Alpha Texture", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.0
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader
	{
		Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _Alpha;

		fixed4 GetColor(float2 uv)
		{
			return 1 - tex2D(_MainTex, float2(uv.x, fmod(uv.y, 1)));
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = GetColor(IN.uv_MainTex);

			for(uint i = 1; i < 2; i++)
			{
				c = c + GetColor(IN.uv_MainTex + float2(0, float(i) / 2.0));
			}

			c /= 2;

			o.Albedo = _Color;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = (1 - c.a) * _Color.a * _Alpha;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
