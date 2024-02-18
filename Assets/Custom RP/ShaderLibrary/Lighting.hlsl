#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

// ������ ���� ���� ���
float3 IncomingLight(Surface surface, Light light) 
{
	// ������ �����, ǥ���� ���� ������ ���� * ���� �� 
	//return dot(surface.normal, light.direction) * light.color;

	// ������ ������ 0���� �����ؾ� ��, saturate �̿�
	return saturate(dot(surface.normal, light.direction ) * light.attenuation) * light.color;
}

float3 GetLighting(Surface surface, BRDF brdf, Light light)
{
	return IncomingLight(surface, light) * DirectBRDF(surface, brdf, light);//* brdf.diffuse;//* surface.color;
}

// ���� ������ ����ϱ� ���� �Լ� ���� 
float3 GetLighting(Surface surfaceWS, BRDF brdf, GI gi)
{
	// �˺��� ���� 
	// ǥ�� ������ ����� ǥ�Խ�Ų�� 
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