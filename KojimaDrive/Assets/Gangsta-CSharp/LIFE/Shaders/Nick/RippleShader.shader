Shader "Custom/RippleShader" {
	Properties{
		/*_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0*/

		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Scale("Scale", float) = 1
		_Speed("Speed", float) = 1
		_Frequency("Frequency", float) = 1
			[HideInInspector]_WaveAmplitude1("WaveAmplitude1", float) = 0
			[HideInInspector]_WaveAmplitude2("WaveAmplitude2", float) = 0
			[HideInInspector]_WaveAmplitude3("WaveAmplitude3", float) = 0
			[HideInInspector]_WaveAmplitude4("WaveAmplitude4", float) = 0
			[HideInInspector]_WaveAmplitude5("WaveAmplitude5", float) = 0
			[HideInInspector]_WaveAmplitude6("WaveAmplitude6", float) = 0
			[HideInInspector]_WaveAmplitude7("WaveAmplitude7", float) = 0
			[HideInInspector]_WaveAmplitude8("WaveAmplitude8", float) = 0

			[HideInInspector]_Distance1("Distance1", float) = 0
			[HideInInspector]_Distance2("Distance2", float) = 0
			[HideInInspector]_Distance3("Distance3", float) = 0
			[HideInInspector]_Distance4("Distance4", float) = 0
			[HideInInspector]_Distance5("Distance5", float) = 0
			[HideInInspector]_Distance6("Distance6", float) = 0
			[HideInInspector]_Distance7("Distance7", float) = 0
			[HideInInspector]_Distance8("Distance8", float) = 0



	}
		SubShader{
			Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+1" "ForceNoShadowCasting" = "True" }
			LOD 200

			CGPROGRAM
			
			#pragma surface surf  Lambert vertex:vert 
			#pragma target 4.0


			sampler2D _MainTex;
		float _Scale, _Speed, _Frequency;
		half4 _Color;
		float _WaveAmplitude1, _WaveAmplitude2, _WaveAmplitude3, _WaveAmplitude4, _WaveAmplitude5, _WaveAmplitude6, _WaveAmplitude7, _WaveAmplitude8;
		float _offsetX1, _offsetZ1, _offsetX2, _offsetZ2, _offsetX3, _offsetZ3, _offsetX4, _offsetZ4, _offsetX5, _offsetZ5, _offsetX6, _offsetZ6, _offsetX7, _offsetZ7, _offsetX8, _offsetZ8;
		float _Distance1, _Distance2, _Distance3, _Distance4, _Distance5, _Distance6, _Distance7, _Distance8;
		float _xImpact1, _zImpact1, _xImpact2, _zImpact2, _xImpact3, _zImpact3, _xImpact4, _zImpact4, _xImpact5, _zImpact5, _xImpact6, _zImpact6, _xImpact7, _zImpact7, _xImpact8, _zImpact8;

			struct Input {
				float2 uv_MainTex;
				float3 customValue;
			};



			void vert(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);

				half offsetvert = ((v.vertex.x * v.vertex.x) + (v.vertex.z * v.vertex.z));
				half offsetvert2 = v.vertex.x + v.vertex.z; //diagonal waves

				half value0 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert2);

				half value1 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _offsetX1) + (v.vertex.z * _offsetZ1));
				half value2 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _offsetX2) + (v.vertex.z * _offsetZ2));
				half value3 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _offsetX3) + (v.vertex.z * _offsetZ3));
				half value4 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _offsetX4) + (v.vertex.z * _offsetZ4));
				half value5 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _offsetX5) + (v.vertex.z * _offsetZ5));
				half value6 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _offsetX6) + (v.vertex.z * _offsetZ6));
				half value7 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _offsetX7) + (v.vertex.z * _offsetZ7));
				half value8 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _offsetX8) + (v.vertex.z * _offsetZ8));

				/*v.vertex.y += value0; //remove for no waves
				v.normal.y += value0; //remove for no waves
				o.customValue += value0;*/


				/*v.vertex.y += value1 * _WaveAmplitude1;
				o.customValue = value1 * _WaveAmplitude1;
				v.vertex.y += value2 * _WaveAmplitude2;
				o.customValue = value2 * _WaveAmplitude2;
				v.vertex.y += value3 * _WaveAmplitude3;
				o.customValue = value3 * _WaveAmplitude3;
				v.vertex.y += value4 * _WaveAmplitude4;
				o.customValue = value4 * _WaveAmplitude4;
				v.vertex.y += value5 * _WaveAmplitude5;
				o.customValue = value5 * _WaveAmplitude5;
				v.vertex.y += value6 * _WaveAmplitude6;
				o.customValue = value6 * _WaveAmplitude6;
				v.vertex.y += value7 * _WaveAmplitude7;
				o.customValue = value7 * _WaveAmplitude7;
				v.vertex.y += value8 * _WaveAmplitude8;
				o.customValue = value8 * _WaveAmplitude8;*/

				v.vertex.y += value1 + _WaveAmplitude1;
				o.customValue = value1 + _WaveAmplitude1;
				v.vertex.y += value2 + _WaveAmplitude2;
				o.customValue = value2 + _WaveAmplitude2;
				v.vertex.y += value3 + _WaveAmplitude3;
				o.customValue = value3 + _WaveAmplitude3;
				v.vertex.y += value4 + _WaveAmplitude4;
				o.customValue = value4 + _WaveAmplitude4;
				v.vertex.y += value5 + _WaveAmplitude5;
				o.customValue = value5 + _WaveAmplitude5;
				v.vertex.y += value6 + _WaveAmplitude6;
				o.customValue = value6 + _WaveAmplitude6;
				v.vertex.y += value7 + _WaveAmplitude7;
				o.customValue = value7 + _WaveAmplitude7;
				v.vertex.y += value8 + _WaveAmplitude8;
				o.customValue = value8 + _WaveAmplitude8;

				/*v.vertex.y += value1 + _WaveAmplitude1;
				o.customValue = value1 * _WaveAmplitude1;
				v.vertex.y += value2 + _WaveAmplitude2;
				o.customValue = value2 * _WaveAmplitude2;
				v.vertex.y += value3 + _WaveAmplitude3;
				o.customValue = value3 * _WaveAmplitude3;
				v.vertex.y += value4 + _WaveAmplitude4;
				o.customValue = value4 * _WaveAmplitude4;
				v.vertex.y += value5 + _WaveAmplitude5;
				o.customValue = value5 * _WaveAmplitude5;
				v.vertex.y += value6 + _WaveAmplitude6;
				o.customValue = value6 * _WaveAmplitude6;
				v.vertex.y += value7 + _WaveAmplitude7;
				o.customValue = value7 * _WaveAmplitude7;
				v.vertex.y += value8 + _WaveAmplitude8;
				o.customValue = value8 * _WaveAmplitude8;*/

				/*v.vertex.y += value1 * _WaveAmplitude1;
				o.customValue = value1 + _WaveAmplitude1;
				v.vertex.y += value2 * _WaveAmplitude2;
				o.customValue = value2 + _WaveAmplitude2;
				v.vertex.y += value3 * _WaveAmplitude3;
				o.customValue = value3 + _WaveAmplitude3;
				v.vertex.y += value4 * _WaveAmplitude4;
				o.customValue = value4 + _WaveAmplitude4;
				v.vertex.y += value5 * _WaveAmplitude5;
				o.customValue = value5 + _WaveAmplitude5;
				v.vertex.y += value6 * _WaveAmplitude6;
				o.customValue = value6 + _WaveAmplitude6;
				v.vertex.y += value7 * _WaveAmplitude7;
				o.customValue = value7 + _WaveAmplitude7;
				v.vertex.y += value8 * _WaveAmplitude8;
				o.customValue = value8 + _WaveAmplitude8;*/

			}

			void surf(Input IN, inout SurfaceOutput o)
			{
				o.Albedo = _Color.rgb;
				o.Normal.y += IN.customValue;
			}

			ENDCG
		}
			FallBack "Diffuse"
}
