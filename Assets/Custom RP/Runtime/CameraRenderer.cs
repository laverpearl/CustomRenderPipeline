using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    // 명령 버퍼
    private const string bufferName = "Render Camera";
    private static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
    private static ShaderTagId litShaderTagId = new ShaderTagId("CustomLit");

    private ScriptableRenderContext context;
    private Camera camera;

    private CullingResults cullingResults;
    private Lighting lighting = new Lighting();
    private CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    // 그리기 
    public void Render(ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing, ShadowSettings shadowSettings)
    {
        this.context = context;
        this.camera = camera;

        PrepareBuffer();
        PrepareForSceneWindow();

        if (!Cull(shadowSettings.maxDistance)) 
        { 
            return; 
        }

        buffer.BeginSample(SampleName);
        ExecuteBuffer();
        lighting.Setup(context, cullingResults, shadowSettings);
        buffer.EndSample(SampleName);

        // 그리기 전에 클리어 
        Setup();
        // 그리기 (RenderLoop, RenderSkybox)
        DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
        DrawUnsupportedShaders();
        DrawGizmos();
        lighting.Cleanup();
        // 작업 제출해야 보인다 
        Submit();

        // 드로우콜 -> 각각의 개체 그리는 수 + 스카이 박스 그리기 1 + 지우기 1
    }

    private void Setup()
    {
        context.SetupCameraProperties(camera);
        CameraClearFlags flags = camera.clearFlags;
        buffer.ClearRenderTarget(
            flags <= CameraClearFlags.Depth,
            flags == CameraClearFlags.Color,
            flags == CameraClearFlags.Color ?
                camera.backgroundColor.linear : Color.clear);

        buffer.BeginSample(SampleName);
        ExecuteBuffer();
    }
    
    // 컬링 / 카메라가 보는 객체만 그리기 
    private bool Cull(float maxShadowDistance)
    {
        if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
        {
            p.shadowDistance = Mathf.Min(maxShadowDistance, camera.farClipPlane);
            cullingResults = context.Cull(ref p);

            return true;
        }

        return false;
    }

    // 카메라가 볼 수 있는 모든 것 그리기 
    private void DrawVisibleGeometry(bool useDynamicBatching, bool useGPUInstancing)
    {
        // 순서 : 불투명, 스카이박스, 투명 그리기 
        // 투명 개체는 깊이 버퍼에 쓰지 않기 때문에 순서가 스카이박스 그리고 나서 그리는 것으로 됨 / 1.2.7 

        var sortingSettings = new SortingSettings(camera);
        var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings)
        {
            enableDynamicBatching = useDynamicBatching,
            enableInstancing = useGPUInstancing,
            perObjectData = PerObjectData.Lightmaps
        };
        drawingSettings.SetShaderPassName(1, litShaderTagId);
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

        // 컬링 결과를 보낸다 
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

        // 스카이 박스
        context.DrawSkybox(camera);

        // criteria 정렬 속성을 설정해서 그리기 순서를 결정할 수 있다 //CommonOpaque
        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;

        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

    private void Submit()
    {
        buffer.EndSample(SampleName);

        ExecuteBuffer();
        context.Submit();
    }

    // 버퍼 실행, 바로 삭제
    private void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}