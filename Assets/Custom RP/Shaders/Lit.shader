Shader "Custom RP/Lit"
{
	Properties
	{
		_BaseMap("Texture", 2D) = "white" {}
		//회색으로
		_BaseColor("Color", Color) = (0.5, 0.5, 0.5, 1.0)
	}

	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "CustomLit" }
				HLSLPROGRAM
				#pragma vertex LitPassVertex
				#pragma fragment LitPassFragment
				#include "LitPass.hlsl"
				ENDHLSL
		}
	}
}
