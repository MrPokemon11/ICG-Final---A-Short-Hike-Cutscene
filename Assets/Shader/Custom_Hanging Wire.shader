Shader "Custom/Hanging Wire" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_Wind1Freq ("Wind 1 Freq", Float) = 0
		_Wind1Amp ("Wind 1 Amp", Float) = 0
		_Wind2Freq ("Wind 2 Freq", Float) = 0
		_Wind2Amp ("Wind 2 Amp", Float) = 0
		_WindPhaseScale ("Wind Phase Scale", Float) = 0
		_Droop ("Droop", Float) = 10
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

			float4 _Color;

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return _Color; // RGBA
			}

			ENDHLSL
		}
	}
}