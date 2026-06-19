using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class CalculateObjectVelocity : VFXSpawnerCallbacks
{
    static readonly int s_positionPropertyId = Shader.PropertyToID("ObjectPositionWS");
    static readonly int s_positionAttributeId = Shader.PropertyToID("position");
    static readonly int s_oldPositionAttributeId = Shader.PropertyToID("oldPosition");
    static readonly int s_velocityAttributeId = Shader.PropertyToID("velocity");

    public class InputProperties
    {
        public Vector3 ObjectPositionWS = Vector3.zero;
    }
    float3 m_lastPosition;

    public override void OnPlay(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
    {
        m_lastPosition = vfxValues.GetVector3(s_positionPropertyId);
    }

    public override void OnUpdate(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
    {
        if (!state.playing || state.deltaTime == 0) return;

        float3 position = vfxValues.GetVector3(s_positionPropertyId);

        state.vfxEventAttribute.SetVector3(s_oldPositionAttributeId, m_lastPosition);
        state.vfxEventAttribute.SetVector3(s_positionAttributeId, position);

        state.vfxEventAttribute.SetVector3(s_velocityAttributeId, (position - m_lastPosition) / state.deltaTime);

        m_lastPosition = position;
    }

    public override void OnStop(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
    {

    }
}