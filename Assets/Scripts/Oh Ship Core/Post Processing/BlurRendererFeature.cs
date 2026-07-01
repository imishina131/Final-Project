using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class BlurRendererFeature : ScriptableRendererFeature
{
    [FormerlySerializedAs("blurMaterial")] public Material m_blurMaterial;
    BlurRenderPass m_pass;

    public override void Create() => m_pass = new BlurRenderPass(m_blurMaterial);

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        => renderer.EnqueuePass(m_pass);
}