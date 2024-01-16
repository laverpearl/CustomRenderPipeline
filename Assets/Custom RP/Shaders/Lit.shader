Shader "Custom RP/Lit"
{
	Properties
	{
		_BaseMap("Texture", 2D) = "white" {}
		_BaseColor("Color", Color) = (0.5, 0.5, 0.5, 1.0)
		_Metallic("Metallic", Range(0, 1)) = 0
		_Smoothness("Smoothness", Range(0, 1)) = 0.5
	}

	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "CustomLit" } // Ŀ���� ���� ��� 
				HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex LitPassVertex
				#pragma fragment LitPassFragment
				#include "LitPass.hlsl"
				ENDHLSL
		}
	}
}
