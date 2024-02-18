using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    // ��� ����
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

    // �׸��� 
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

        // �׸��� ���� Ŭ���� 
        Setup();
        // �׸��� (RenderLoop, RenderSkybox)
        DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
        DrawUnsupportedShaders();
        DrawGizmos();
        lighting.Cleanup();
        // �۾� �����ؾ� ���δ� 
        Submit();

        // ��ο��� -> ������ ��ü �׸��� �� + ��ī�� �ڽ� �׸��� 1 + ����� 1
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
    
    // �ø� / ī�޶� ���� ��ü�� �׸��� 
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

    // ī�޶� �� �� �ִ� ��� �� �׸��� 
    private void DrawVisibleGeometry(bool useDynamicBatching, bool useGPUInstancing)
    {
        // ���� : ������, ��ī�̹ڽ�, ���� �׸��� 
        // ���� ��ü�� ���� ���ۿ� ���� �ʱ� ������ ������ ��ī�̹ڽ� �׸��� ���� �׸��� ������ �� / 1.2.7 

        var sortingSettings = new SortingSettings(camera);
        var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings)
        {
            enableDynamicBatching = useDynamicBatching,
            enableInstancing = useGPUInstancing,
            perObjectData = PerObjectData.Lightmaps
        };
        drawingSettings.SetShaderPassName(1, litShaderTagId);
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

        // �ø� ����� ������ 
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

        // ��ī�� �ڽ�
        context.DrawSkybox(camera);

        // criteria ���� �Ӽ��� �����ؼ� �׸��� ������ ������ �� �ִ� //CommonOpaque
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

    // ���� ����, �ٷ� ����
    private void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}