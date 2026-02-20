Shader "Custom Unlit/Unlit Ocean Water (World Space)" {
	Properties {
		_Color ("Water Color", Vector) = (1,1,1,1)
		_DistortTex ("Distortion Normals", 2D) = "white" {}
		_RippleSpeed ("Distortion Scroll Speed", Float) = 0.1
		_MainTex ("Water Edge Map", 2D) = "white" {}
		_EdgeColorR ("Water Edge Color R", Vector) = (1,1,1,1)
		_EdgeColorG ("Water Edge Color G", Vector) = (1,1,1,1)
		_EdgeColorB ("Water Edge Color B", Vector) = (1,1,1,1)
		_EdgeColorA ("Water Edge Color A", Vector) = (1,1,1,1)
		_EdgeDistortStrength ("Edge Distortion Strength", Range(-1, 1)) = 0
		_FoamColor ("Foam Color", Vector) = (1,1,1,1)
		_FoamThickness ("Foam Thickness", Range(-1, 8)) = 0
		_BehindFoamCutoff ("Behind Foam Cutoff", Range(-10, 10)) = 4
		_FoamDistortStrength ("Foam Distortion Strength", Range(-8, 8)) = 0
		_WhiteFoamCutoff ("White Foam Cutoff", Range(0, 1)) = 0.9
		_WhiteFoamColor ("White Foam Color", Vector) = (1,1,1,1)
		_RippleTex ("Ripple Texture", 2D) = "white" {}
		_RippleAlphaCutoff ("Ripple Alpha Cutoff", Range(-1, 1)) = 0
		_DistortStrength ("Ripple Distortion Strength", Range(-1, 1)) = 0
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