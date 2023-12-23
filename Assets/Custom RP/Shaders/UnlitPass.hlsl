#ifndef CUSTOM_UNLIT_PASS_INCLUDED
#define CUSTOM_UNLIT_PASS_INCLUDED

float4 UnlitPassVertex() : SV_POSITION
{
	return 0.0;
}

float4 UnlitPassFragment() : SV_TARGET
{
	return 0.0;
}

#endif

Shader "Custom RP/Unlit"
{
	Properties
	{
	}

	SubShader
	{
		Pass
		{
			HLSLPROGRAM
			#pragma vertex UnlitPassVertex
			#pragma fragment UnlitPassFragment
			#include "UnlitPass.hlsl"
			ENDHLSL
		}
	}
}