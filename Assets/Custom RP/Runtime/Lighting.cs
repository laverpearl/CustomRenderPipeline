using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting
{
    const int maxDirLightCount = 4;

    private static readonly int
        dirLightCountId = Shader.PropertyToID("_DirectionalLightCount"),
        dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors"),
        dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");

    private static readonly Vector4[]
        dirLightColors = new Vector4[maxDirLightCount],
        dirLightDirections = new Vector4[maxDirLightCount];

    const string bufferName = "Lighting";
    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    CullingResults cullingResults;

    public void Setup(ScriptableRenderContext context, CullingResults cullingResults)
    {
        this.cullingResults = cullingResults;
        buffer.BeginSample(bufferName);
        SetupLights();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    void SetupDirectionalLight(int index, ref VisibleLight visibleLight)
    {
        dirLightColors[index] = visibleLight.finalColor;
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
    }

    void SetupLights() 
    {
        NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;

        // 모든 가시광선을 반복하면서 세팅 + GPU에 데이터를 보내기 
        //for (int i = 0; i < visibleLights.Length; i++)
        //{
        //    VisibleLight visibleLight = visibleLights[i];
        //    SetupDirectionalLight(i, visibleLight);
        //}

        //buffer.SetGlobalInt(dirLightCountId, visibleLights.Length);
        //buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
        //buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);

        int dirLightCount = 0;
        for (int i = 0; i < visibleLights.Length; i++)
        {
            VisibleLight visibleLight = visibleLights[i];
            if (visibleLight.lightType == LightType.Directional) // 방향성 이외에 다른 조명은 무시하도록 함 
            {
                this.SetupDirectionalLight(dirLightCount++, ref visibleLight);
                if (dirLightCount >= maxDirLightCount)
                {
                    break;
                }
            }
        }

        buffer.SetGlobalInt(dirLightCountId, visibleLights.Length);
        buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
        buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);
    }
}