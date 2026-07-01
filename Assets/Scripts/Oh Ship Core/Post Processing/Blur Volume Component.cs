using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[Serializable, VolumeComponentMenu("Post-processing/Custom Blur")]
public class BlurVolumeComponent : VolumeComponent, IPostProcessComponent
{
    [FormerlySerializedAs("spread")] public ClampedFloatParameter Spread = new(0f, 0f, 20f);
    [FormerlySerializedAs("gridSize")] public ClampedIntParameter GridSize = new(1, 1, 33);
    public bool IsActive() => Spread.value > 0f;
    public bool IsTileCompatible() => false;
}