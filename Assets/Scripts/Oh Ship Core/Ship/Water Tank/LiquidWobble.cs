using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class LiquidWobble : MonoBehaviour
{
    static readonly int s_wobbleX = Shader.PropertyToID("_X_Wobble");
    static readonly int s_wobbleZ = Shader.PropertyToID("_Z_Wobble");
    
    Renderer m_rend;
    Vector3 m_lastPos;
    Vector3 m_velocity;
    Quaternion m_lastRotation;
    Vector3 m_angularVelocityVec; 
    
    public float MaxWobble = 0.3f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    public float LinearJoltSensitivity = 0.5f;
    public float AngularJoltSensitivity = 0.5f;
    public float VelocitySensitivity = 0.02f;
    public float AngularSensitivity = 0.1f;
    
    float m_wobbleAmountX;
    float m_wobbleAmountZ;
    float m_wobbleAmountToAddX;
    float m_wobbleAmountToAddZ;
    float m_pulse;
    float m_time = 0.5f;
    
    void Start()
    {
        m_rend = GetComponent<Renderer>();
        m_lastPos = transform.position;
        m_lastRotation = transform.rotation;
    }

    void Update()
    {
        if(Time.timeScale <= 0 || Time.deltaTime <= 0)
        {
            m_rend.material.SetFloat(s_wobbleX, 0f);
            m_rend.material.SetFloat(s_wobbleZ, 0f);
            m_lastPos = transform.position;
            m_lastRotation = transform.rotation;
            return;
        }
        m_time += Time.deltaTime;
        m_wobbleAmountToAddX = Mathf.Lerp(m_wobbleAmountToAddX, 0, Time.deltaTime * Recovery);
        m_wobbleAmountToAddZ = Mathf.Lerp(m_wobbleAmountToAddZ, 0, Time.deltaTime * Recovery);
        m_pulse = 2 * Mathf.PI * WobbleSpeed;
        m_wobbleAmountX = m_wobbleAmountToAddX * Mathf.Sin(m_pulse * m_time);
        m_wobbleAmountZ = m_wobbleAmountToAddZ * Mathf.Sin(m_pulse * m_time);
        m_rend.material.SetFloat(s_wobbleX, m_wobbleAmountX);
        m_rend.material.SetFloat(s_wobbleZ, m_wobbleAmountZ);
        Vector3 newVelocity = (transform.position - m_lastPos) / Time.deltaTime;
        Vector3 velocityChange = (newVelocity - m_velocity);
        Vector3 localVelocity = transform.InverseTransformDirection(newVelocity);
        Vector3 localVelocityChange = transform.InverseTransformDirection(velocityChange);
        Quaternion rotDifference = transform.rotation * Quaternion.Inverse(m_lastRotation);
        rotDifference.ToAngleAxis(out float angleVel, out Vector3 axisVel);
        Vector3 newAngularVelocityVec = (axisVel * angleVel) / Time.deltaTime;
        Vector3 angularVelocityChangeVec = newAngularVelocityVec - m_angularVelocityVec;
        Vector3 localAngularVel = transform.InverseTransformDirection(newAngularVelocityVec);
        Vector3 localAngularVelChange = transform.InverseTransformDirection(angularVelocityChangeVec);
        float linearVelocityToAddX = localVelocity.x * VelocitySensitivity + localVelocityChange.x * LinearJoltSensitivity;
        float linearVelocityToAddZ = localVelocity.z * VelocitySensitivity + localVelocityChange.z * LinearJoltSensitivity;
        float angularVelocityToAddX = (localAngularVelChange.x * 0.2f * AngularJoltSensitivity) + (localAngularVel.x * 0.2f * AngularSensitivity);
        float angularVelocityToAddZ = (localAngularVelChange.z * 0.2f * AngularJoltSensitivity) + (localAngularVel.z * 0.2f * AngularSensitivity);
        m_wobbleAmountToAddX = Mathf.Clamp(
            m_wobbleAmountToAddX + linearVelocityToAddX + angularVelocityToAddX * Time.deltaTime,
            -MaxWobble, MaxWobble
        );
        m_wobbleAmountToAddZ = Mathf.Clamp(
            m_wobbleAmountToAddZ + linearVelocityToAddZ + angularVelocityToAddZ * Time.deltaTime,
            -MaxWobble, MaxWobble
        );
        m_lastPos = transform.position;
        m_lastRotation = transform.rotation;
        m_velocity = newVelocity;
        m_angularVelocityVec = newAngularVelocityVec;
    }
}