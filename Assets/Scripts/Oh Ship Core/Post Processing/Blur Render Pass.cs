using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class BlurRenderPass : ScriptableRenderPass
{
    static readonly int s_spread = Shader.PropertyToID("_Spread");
    static readonly int s_gridSize = Shader.PropertyToID("_GridSize");
    readonly Material m_material;
    class PassData
    {
        public TextureHandle Source;
        public Material Material;
        public int PassIndex;
    }

    public BlurRenderPass(Material material)
    {
        m_material = material;
        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        BlurVolumeComponent blur = VolumeManager.instance.stack.GetComponent<BlurVolumeComponent>();
        if (!blur.IsActive()) return;

        m_material.SetFloat(s_spread, blur.Spread.value);
        m_material.SetInteger(s_gridSize, blur.GridSize.value);

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        TextureHandle source = resourceData.activeColorTexture;

        TextureDesc desc = renderGraph.GetTextureDesc(source);
        desc.name = "_BlurTemp";
        desc.clearBuffer = false;
        TextureHandle tempTarget = renderGraph.CreateTexture(desc);

        // Horizontal: source -> tempTarget
        using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass<PassData>("Blur Horizontal", out PassData passData))
        {
            passData.Source = source;
            passData.Material = m_material;
            passData.PassIndex = 0;

            builder.UseTexture(source);
            builder.SetRenderAttachment(tempTarget, 0);

            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                Blitter.BlitTexture(ctx.cmd, data.Source, new(1, 1, 0, 0), data.Material, data.PassIndex);
            });
        }

        // Vertical: tempTarget -> source
        using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass<PassData>("Blur Vertical", out var passData))
        {
            passData.Source = tempTarget;
            passData.Material = m_material;
            passData.PassIndex = 1;

            builder.UseTexture(tempTarget);
            builder.SetRenderAttachment(source, 0);

            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                Blitter.BlitTexture(ctx.cmd, data.Source, new Vector4(1, 1, 0, 0), data.Material, data.PassIndex);
            });
        }
    }
}