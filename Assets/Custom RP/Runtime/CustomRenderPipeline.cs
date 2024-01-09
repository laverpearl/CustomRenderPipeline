using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    CameraRenderer renderer = new CameraRenderer();
    private readonly bool useDynamicBatching;
    private readonly bool useGPUInstancing;

    public CustomRenderPipeline(bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher)
    {
        this.useDynamicBatching = useDynamicBatching;
        this.useGPUInstancing = useGPUInstancing;
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
        GraphicsSettings.lightsUseLinearIntensity = true;
    }

    // Unity 2022 ������ ����ϴ� �Լ������� abstract�� ����Ǿ� �����Ƿ� ������ �д�.
    protected override void Render(ScriptableRenderContext context, Camera[] cameras) 
    { 
    }

    protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
    {
        foreach (var camera in cameras)
        {
            renderer.Render(context, camera, this.useDynamicBatching, this.useGPUInstancing);
        }
    }
}