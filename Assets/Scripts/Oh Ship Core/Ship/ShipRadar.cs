using System;
using MatrixUtils.Attributes;
using UnityEngine;
/// <summary>
/// A simple radar system for a ship that uses a <see cref="ComputeShader"/> to render a render texture with radar data.
/// </summary>
public class ShipRadar : MonoBehaviour
{
    static readonly int s_uv = Shader.PropertyToID("_UV");
    static readonly int s_decayRate = Shader.PropertyToID("_DecayRate");
    static readonly int s_radarTexture = Shader.PropertyToID("_RadarTexture");
    static readonly int s_sweepAngle = Shader.PropertyToID("_SweepAngle");
    static readonly int s_sweepWidth = Shader.PropertyToID("_SweepWidth");
    
    [SerializeField, RequiredField] ComputeShader m_radarShader;
    [SerializeField, RequiredField] RenderTexture m_radarDataTexture;
    [SerializeField] float m_degreesPerSecond;
    [SerializeField] float m_maxDistance;
    [SerializeField] float m_decayRate;
    [SerializeField] float m_sweepWidth;
    float m_sweepAngle;
    int m_decayKernel;
    int m_writeKernel;
    int m_sweepKernel;
    void Start()
    {
        m_decayKernel = m_radarShader.FindKernel("Decay");
        m_writeKernel = m_radarShader.FindKernel("WriteHit");
        m_sweepKernel = m_radarShader.FindKernel("WriteSweep");
        m_radarShader.SetTexture(m_decayKernel, s_radarTexture, m_radarDataTexture);
        m_radarShader.SetTexture(m_writeKernel, s_radarTexture, m_radarDataTexture);
        m_radarShader.SetTexture(m_sweepKernel, s_radarTexture, m_radarDataTexture);
        m_radarShader.SetFloat(s_decayRate, m_decayRate);
        m_radarShader.SetFloat(s_sweepWidth, m_sweepWidth);
    }
    void FixedUpdate()
    {
        m_sweepAngle += m_degreesPerSecond * Time.deltaTime;
        if (m_sweepAngle > 360f) m_sweepAngle -= 360f;
        float sweepRad = m_sweepAngle * Mathf.Deg2Rad;
        Vector3 direction = new(Mathf.Cos(sweepRad), 0, Mathf.Sin(sweepRad));
        m_radarShader.SetFloat(s_sweepAngle, sweepRad);
        m_radarShader.Dispatch(m_sweepKernel, m_radarDataTexture.width / 8, m_radarDataTexture.height / 8, 1);
        m_radarShader.Dispatch(m_decayKernel, m_radarDataTexture.width / 8, m_radarDataTexture.height / 8, 1);
        if(!Physics.Raycast(transform.position, direction, out RaycastHit hit, m_maxDistance))
        {
            Debug.DrawRay(transform.position, direction * m_maxDistance, Color.red);
            return;
        }
        Debug.DrawRay(transform.position, direction * hit.distance, Color.green);
        Vector2 offset = new(-(hit.point.x - transform.position.x), -(hit.point.z - transform.position.z));
        Vector2 uv = (offset / m_maxDistance) * 0.5f + Vector2.one * 0.5f;
        m_radarShader.SetVector(s_uv, uv);
        m_radarShader.Dispatch(m_writeKernel, 1, 1, 1);
    }
}
