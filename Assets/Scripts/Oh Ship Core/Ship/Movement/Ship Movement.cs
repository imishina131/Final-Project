using MatrixUtils.GenericDatatypes;
using UnityEngine;
using UnityEngine.Serialization;

public class ShipMovement : MonoBehaviour
{
    [field:SerializeField]public Observer<float> Rudder{ get; private set;}
    [field:SerializeField]public Observer<float> Throttle{ get; private set;}

    [SerializeField] Rigidbody m_rigidbody;
    [SerializeField] Transform m_wheelPowerPoint;
    [SerializeField] Transform m_rudderPoint;
    [SerializeField] AnimationCurve m_rudderEffectiveness;
    [SerializeField] AnimationCurve m_throttleEffectiveness;
    
    [SerializeField] private float m_rudderTurnMultiplier = 10;
    public void SetRudder(float rudder) => Rudder.Value = Mathf.Clamp(rudder, -1 , 1);
    public void SetThrottle(float throttle)
    {
        Throttle.Value = Mathf.Clamp(throttle, -1, 1);
    }
    
    private SteamPressureSystem m_steamPressureSystem;
    private float m_steamPressure;

    void Start()
    {
        m_steamPressureSystem = GetComponent<SteamPressureSystem>();
        if (m_steamPressureSystem == null)
        {
            Debug.LogWarning("No SteamPressureSystem found on ship!");
        }
        Throttle.Notify();
        Rudder.Notify();
    }
    
    void FixedUpdate()
    {
        if (!m_steamPressureSystem)
        {
            Debug.LogWarning("No SteamPressureSystem found on ship!");
            m_steamPressure = 1;
        }
        else
        {
            m_steamPressure = m_steamPressureSystem.SteamPressure;
        }

        m_rigidbody.AddForceAtPosition(m_wheelPowerPoint.forward * (m_throttleEffectiveness.Evaluate(Throttle * 2) * (m_steamPressure)), m_wheelPowerPoint.position, ForceMode.Force);
       // Debug.Log(m_steamPressureSystem.SteamPressure);
         //Debug.Log($"Throttle: {Throttle}, Pressure: {m_steamPressureSystem.SteamPressure}, Force: {m_throttleEffectiveness.Evaluate(Throttle * 2) * (m_steamPressure)}");
        ApplyRudderForce(Rudder);
    }

    void ApplyRudderForce(float rudderInput)
    {
        Vector3 flatForward = Vector3.ProjectOnPlane(m_rigidbody.transform.forward, Vector3.up).normalized;
        Vector3 flatLeft = Vector3.ProjectOnPlane(-m_rigidbody.transform.right, Vector3.up).normalized;
        float forwardSpeed = Vector3.Dot(m_rigidbody.linearVelocity, flatForward);
        float rudderEffectiveness = m_rudderEffectiveness.Evaluate(forwardSpeed);
        Vector3 rudderForce = flatLeft * (rudderInput * rudderEffectiveness);
        m_rigidbody.AddForceAtPosition(rudderForce * (m_rudderTurnMultiplier * 100), m_rudderPoint.position, ForceMode.Force);
        
    }
}
