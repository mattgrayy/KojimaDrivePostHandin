Shader "Image Effects/RadialBlurFilter" {

	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_SampleDist("SampleDist", Float) = 1
		_SampleStrength("SampleStrength", Float) = 2.2
		_SamplePosX("SamplePosX", Float) = 0.5
		_SamplePosY("SamplePosY", Float) = 0.5
	}

		SubShader
	{
		Pass
	{
		ZTest Always Cull Off ZWrite Off
		Fog{ Mode off }

		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360 because it uses wrong array syntax (type[size] name)
//#pragma exclude_renderers d3d11 xbox360
// Upgrade NOTE: excluded shader from DX11, Xbox360, OpenGL ES 2.0 because it uses unsized arrays
//#pragma exclude_renderers d3d11 xbox360 gles
		//#pragma exclude_renderers d3d11 xbox360 gles
		//#pragma exclude_renderers xbox360
#pragma debug
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest

		#include "UnityCG.cginc"

				uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _MainTex_TexelSize;
			float _SampleDist;
			float _SampleStrength;
			float _SamplePosX;
			float _SamplePosY;

			struct v2f {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_img v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				float2 texCoord = i.uv;

		#if SHADER_API_D3D9
				if (_MainTex_TexelSize.y < 0)
					texCoord.y = 1 - texCoord.y;
		#endif
				float2 spos = float2(_SamplePosX, _SamplePosY);
				float2 dir = spos - texCoord;
				float dist = length(dir);

				dir /= dist;

				float4 color = tex2D(_MainTex, texCoord);
				float4 sum = color;
				float samples[10] = { -0.08,-0.05,-0.03,-0.02,-0.01,0.01,0.02,0.03,0.05,0.08 };

				for (int i = 0; i < 10; ++i)
				{
					sum += tex2D(_MainTex, texCoord + dir * samples[i] * _SampleDist);
				}

				sum *= 1.0 / 11.0;
				float t = saturate(dist * _SampleStrength);
				return lerp(color, sum, t);
			}
				ENDCG
			}
	}
		Fallback off
}
// [Unity3D] Radial Blur Effect Shader | EjongHyuck