Shader "Custom/Birch" {
	Properties {
		_Type ("Bark Type", Range(0,1)) = 0.0
		_Noise ("Noise Properties", Vector) = (1, 1, 1, 1)
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		#include "Assets/Shaders/SimplexNoise.cginc"

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		float4 _Noise;
		float _Type;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float3 birch(float3 worldPos) {
			float3 color;

			float3 samplePos = float3(worldPos.x, worldPos.y * 3, worldPos.z);
			float noise = snoise(samplePos * _Noise.y) * _Noise.x;

			if(noise > 0.8f) {
				color = float3(0, 0, 0);
			} else {
				color = float3(1, 1, 1);
			}

			float3 samplePos2 = float3(worldPos.x, worldPos.y * _Noise.z, worldPos.z);
			float noise2 = snoise(samplePos2 * 3.31f) * 1.05f;

			if(noise2 > _Noise.w) {
				color = float3(0, 0, 0);
			}

			return color;
		}

		// Not used, implemented a separate shader for this
		float3 oak(float3 worldPos) {
			float3 color;

			float3 samplePos = float3(worldPos.x * 5, worldPos.y, worldPos.z * 5);
			float noise = snoise(samplePos * _Noise.y) * _Noise.x;
			noise = abs(noise);
			noise = 1 - noise;
			noise = (noise + 1) / 2;
			//noise = noise * noise;

			if(noise < 0.1f) {
				color = float3(0.2, 0.05, 0);
			} else if(noise < 0.2f) {
				color = float3(0.3, 0.1, 0);
			} else if(noise < 0.4f) {
				color = float3(0.4, 0.15, 0.1);
			} else if(noise < 0.6f) {
				color = float3(0.5, 0.2, 0.2);
			} else {
				color = float3(1, 0.7, 0.3);
			}

			//float3 samplePos2 = float3(worldPos.x, worldPos.y * 6, worldPos.z);
			//float noise2 = snoise(samplePos2 * 3.31f) * 1.05f;
			//if(noise2 > 0.7f) {
			//	color = float3(0, 0, 0);
			//}

			return color;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			float3 samplePos = float3(IN.worldPos.x, IN.worldPos.y * 3, IN.worldPos.z);
			float noise = snoise(samplePos * _Noise.y) * _Noise.x;

			float3 color = birch(IN.worldPos.xyz);

			o.Albedo = color;

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
