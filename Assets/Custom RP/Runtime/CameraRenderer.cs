using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    ScriptableRenderContext context;

    Camera camera;

    const string bufferName = "Render Camera";

    private CullingResults cullingResults;

    private static Material errorMaterial;

    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

    private CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        this.context = context;
        this.camera = camera;

        this.PrepareBuffer();
        this.PrepareForSceneWindow();

        if (!this.Cull())
        {
            return;
        }

        this.Setup();
        this.DrawVisibleGeometry();
        this.DrawUnsupportedShaders();
        this.DrawGizmos();
        this.Submit();
    }

    private void Setup()
    {
        this.context.SetupCameraProperties(camera);
        CameraClearFlags flags = camera.clearFlags;
        this.buffer.ClearRenderTarget(
            flags <= CameraClearFlags.Depth,
            flags <= CameraClearFlags.Color,
            flags == CameraClearFlags.Color ?
                            camera.backgroundColor.linear : Color.clear);
        this.buffer.BeginSample(SampleName);
        this.ExecuteBuffer();
    }

    private void DrawVisibleGeometry()
    {
        var sortingSettings = new SortingSettings(this.camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };
        var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

        this.context.DrawRenderers(
            cullingResults, ref drawingSettings, ref filteringSettings);

        context.DrawSkybox(camera);

        //sortingSettings.criteria = SortingCriteria.CommonTransparent;
        //drawingSettings.sortingSettings = sortingSettings;
        //filteringSettings.renderQueueRange = RenderQueueRange.transparent;

        //context.DrawRenderers(
        //    cullingResults, ref drawingSettings, ref filteringSettings);
    }

    private void Submit()
    {
        this.buffer.EndSample(SampleName);
        //this.buffer.EndSample(bufferName);
        this.ExecuteBuffer();
        this.context.Submit();
    }

    private void ExecuteBuffer()
    {
        this.context.ExecuteCommandBuffer(buffer);
        this.buffer.Clear();
    }

    private bool Cull()
    {
        if (camera.TryGetCullingParameters(out var p))
        {
            cullingResults = this.context.Cull(ref p);
            return true;
        }
        return false;
    }
}