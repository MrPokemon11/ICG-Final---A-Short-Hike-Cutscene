Shader "Custom Unlit/Unlit Water (River)" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainTexSpeed ("Main Texture Scroll Speed", Range(-50, 50)) = 1
		_AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
		_FoamColor ("Foam Color", Vector) = (1,1,1,1)
		_FoamDetailTex ("Foam Detail Texture", 2D) = "black" {}
		_FoamDetailCutoff ("Foam Detail Cutoff", Range(0, 1)) = 0.5
		_FoamDetailSpeed ("Foam Detail Scroll Speed", Range(-50, 50)) = 1
		_FoamCurveCutoff ("Curve Foam Cutoff", Range(0, 1)) = 0.5
		_Noise ("Noise Texture", 2D) = "white" {}
		_NoiseSpeed ("Noise Scroll Speed", Range(-10, 10)) = 0.5
		_WaveHeight ("Wave Height", Float) = 0.05
		_WaveSpeed ("Wave Speed", Float) = 20
		_WaveLengthX ("Wave Length X", Float) = 1
		_WaveLengthY ("Wave Length Y", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			float4 _Color;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy) * _Color;
			}

			ENDHLSL
		}
	}
	Fallback "Diffuse"
}