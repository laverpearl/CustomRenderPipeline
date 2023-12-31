Shader "Custom RP/Lit"
{
	Properties
	{
		_BaseMap("Texture", 2D) = "white" {}
		_BaseColor("Color", Color) = (0.5, 0.5, 0.5, 1.0)
	}

	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "CustomLit" } // 커스텀 조명 사용 
				HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex LitPassVertex
				#pragma fragment LitPassFragment
				#include "LitPass.hlsl"
				ENDHLSL
		}
	}
}
