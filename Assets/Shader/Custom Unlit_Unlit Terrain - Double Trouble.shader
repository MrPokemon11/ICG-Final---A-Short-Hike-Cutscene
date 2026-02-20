Shader "Custom Unlit/Unlit Terrain - Double Trouble" {
	Properties {
		_Splat3 ("Layer 3 (A)", 2D) = "white" {}
		_Splat2 ("Layer 2 (B)", 2D) = "white" {}
		_Splat1 ("Layer 1 (G)", 2D) = "white" {}
		_Splat0 ("Layer 0 (R)", 2D) = "white" {}
		_Splat7 ("Layer 7 (A)", 2D) = "white" {}
		_Splat6 ("Layer 6 (B)", 2D) = "white" {}
		_Splat5 ("Layer 5 (G)", 2D) = "white" {}
		_Splat4 ("Layer 4 (R)", 2D) = "white" {}
		_AttenuationThreshold ("Attenuation Threshold", Range(-1, 1)) = 0
		_ShadowBitDepth ("Shadow Bit Depth", Range(0, 8)) = 1
		_SideTex ("Albedo Top (RGB)", 2D) = "white" {}
		_Blend ("Sharpness", Range(0, 150)) = 0.5
		_AltSideTex ("Second Albedo Top (RGB)", 2D) = "white" {}
		_AltSideTexChannel ("Second Albedo Channel", Vector) = (0,0,0,0)
		_AltSideTexChannel2 ("Second Albedo Channel 2", Vector) = (0,0,0,0)
		_Control1 ("First Control Map", 2D) = "black" {}
		_Control2 ("Second Control Map", 2D) = "black" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return float4(1.0, 1.0, 1.0, 1.0); // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "Diffuse"
}