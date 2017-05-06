Shader "Post Processing/Cutoff FX"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_TransitionTex("Transition Texture", 2D) = "white" {}
		_Color("Screen Colour", Color) = (1,1,1,1)
		_Color2("Middle Colour", Color) = (1,1,1,1)
		_Color3("Inner Colour", Color) = (1,1,1,1)
		_Cutoff("Cutoff", Range(0, 10)) = 0
		_CutoffFade("Cutoff Fade", Range(0, 1)) = 0
		_CutoffMiddle("Cutoff Middle", Range(0, 1)) = 0
		_CutoffInner("Cutoff Inner", Range(0, 1)) = 0
		[MaterialToggle] _Distort("Distort", Float) = 0
		_Fade("Fade", Range(0, 1)) = 0
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
					float2 uv1 : TEXCOORD1;
					float4 vertex : SV_POSITION;
				};

				float4 _MainTex_TexelSize;

				v2f simplevert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = v.uv;
					return o;
				}

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = v.uv;
					o.uv1 = v.uv;

					#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						o.uv1.y = 1 - o.uv1.y;
					#endif

					return o;
				}

				sampler2D _TransitionTex;
				int _Distort;
				float _Fade;

				sampler2D _MainTex;
				float _Cutoff;
				float _CutoffFade;
				float _CutoffMiddle;
				float _CutoffInner;
				fixed4 _Color;
				fixed4 _Color2;
				fixed4 _Color3;

				fixed4 simplefrag(v2f i) : SV_Target
				{
					if (i.uv.x < _Cutoff)
						return _Color;

					return tex2D(_MainTex, i.uv);
				}

				fixed4 simplefragopen(v2f i) : SV_Target
				{
					if (0.5 - abs(i.uv.y - 0.5) < abs(_Cutoff) * 0.5)
						return _Color;

					return tex2D(_MainTex, i.uv);
				}

				fixed4 simpleTexture(v2f i) : SV_Target
				{
					fixed4 transit = tex2D(_TransitionTex, i.uv);

					if (transit.b < _Cutoff)
						return _Color;

					return tex2D(_MainTex, i.uv);
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 transit = tex2D(_TransitionTex, i.uv1);

					fixed2 direction = float2(0,0);
					if(_Distort)
						direction = normalize(float2((transit.r - 0.5) * 2, (transit.g - 0.5) * 2));

					fixed4 col = tex2D(_MainTex, i.uv + _Cutoff * direction);

					//if (transit.b < _Cutoff - _CutoffFade) {
					//	float lerp_low = _Cutoff - transit.b;
					//	float lerp_high = _CutoffFade * 2;
					//	fixed4 lerpcolor = lerp(col, _Color2, clamp(lerp_low / lerp_high, 0, 1));
					//	return col = lerp(col, lerpcolor, _Fade);
					//}

					if (transit.b < _Cutoff)
					{
						float lerp_low = _Cutoff - transit.b;
						float lerp_high = _CutoffFade;
						fixed4 lerpcolor = lerp(col, _Color, clamp(lerp_low / lerp_high, 0, 1));
						lerp_low = lerp_low - _CutoffFade;
						lerp_high = _CutoffMiddle;
						lerpcolor = lerp(lerpcolor, _Color2, clamp(lerp_low / lerp_high, 0, 1));
						lerp_low = lerp_low - _CutoffFade;
						lerp_high = _CutoffInner;
						lerpcolor = lerp(lerpcolor, _Color3, clamp(lerp_low / lerp_high, 0, 1));
						return col = lerp(col, lerpcolor, _Fade);
					}

					return col;
				}					
				ENDCG
			}
		}
}
