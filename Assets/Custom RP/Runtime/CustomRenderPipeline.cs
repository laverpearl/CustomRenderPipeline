using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    private CameraRenderer renderer = new CameraRenderer();
    private readonly bool useDynamicBatching;
    private readonly bool useGPUInstancing;
    private ShadowSettings shadowSettings;

    public CustomRenderPipeline(bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher, ShadowSettings shadowSettings)
    {
        this.useDynamicBatching = useDynamicBatching;
        this.useGPUInstancing = useGPUInstancing;
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
        GraphicsSettings.lightsUseLinearIntensity = true;

        this.shadowSettings = shadowSettings;
    }

    // Unity 2022 이전에 사용하던 함수이지만 abstract로 선언되어 있으므로 유지해 둔다.
    protected override void Render(ScriptableRenderContext context, Camera[] cameras) 
    { 
    }

    protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
    {
        // 루프의 모든 카메라 렌더링 
        foreach (var camera in cameras)
        {
            renderer.Render(context, camera, this.useDynamicBatching, this.useGPUInstancing, shadowSettings);
        }
    }
}