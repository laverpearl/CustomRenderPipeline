#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

// 들어오는 빛의 양을 계산
float3 IncomingLight(Surface surface, Light light) 
{
	// 임의의 방향과, 표면의 법선 방향의 내적 * 빛의 색 
	//return dot(surface.normal, light.direction) * light.color;

	// 내적이 음수면 0으로 고정해야 함, saturate 이용
	return saturate(dot(surface.normal, light.direction ) * light.attenuation) * light.color;
}

float3 GetLighting(Surface surface, BRDF brdf, Light light)
{
	return IncomingLight(surface, light) * DirectBRDF(surface, brdf, light);//* brdf.diffuse;//* surface.color;
}

// 실제 조명을 계산하기 위한 함수 생성 
float3 GetLighting(Surface surfaceWS, BRDF brdf, GI gi)
{
	// 알베도 적용 
	// 표면 색상을 결과에 표함시킨다 
	//return surface.normal.y * surface.color;

	ShadowData shadowData = GetShadowData(surfaceWS);

	float3 color = gi.diffuse;
	for (int i = 0; i < GetDirectionalLightCount(); i++) 
	{
		Light light = GetDirectionalLight(i, surfaceWS, shadowData);
		color += GetLighting(surfaceWS, brdf, light);
	}

	return color;
}
#endif