using System.Timers;
using MatrixUtils.GenericDatatypes;
using UnityEngine;
using UnityEngine.Serialization;

public class ShipMovement : MonoBehaviour
{
    [field:SerializeField]public Observer<float> Rudder{ get; private set;}
    [field:SerializeField]public Observer<float> Throttle{ get; private set;}
    [FormerlySerializedAs("m_rigidbody")] public Rigidbody Rigidbody;
    [SerializeField] Transform m_wheelPowerPoint;
    [SerializeField] Transform m_rudderPoint;
    [SerializeField] AnimationCurve m_rudderEffectiveness;
    [SerializeField] AnimationCurve m_throttleEffectiveness;
    [SerializeField] float m_rudderTurnMultiplier = 10;
    [SerializeField] float m_enginePower = 1;
    public void SetRudder(float rudder) => Rudder.Value = Mathf.Clamp(rudder, -1 , 1);
    public void SetThrottle(float throttle) => Throttle.Value = Mathf.Clamp(throttle, -1, 1);
    public void SetEnginePower(float power) => m_enginePower += power = Mathf.Clamp01(power);
    void Start()
    {
        Throttle.Notify();
        Rudder.Notify();
    }

    private void Update()
    {
        SetEnginePower(-Time.deltaTime);
    }
    void FixedUpdate()
    {
        Rigidbody.AddForceAtPosition(m_wheelPowerPoint.forward * (m_throttleEffectiveness.Evaluate(Throttle * 2) * m_enginePower), m_wheelPowerPoint.position, ForceMode.Force);
       // Debug.Log(m_waterController.SteamPressure);
       //Debug.Log($"Throttle: {Throttle}, Pressure: {m_waterController.SteamPressure}, Force: {m_throttleEffectiveness.Evaluate(Throttle * 2) * (m_steamPressure)}");
        ApplyRudderForce(Rudder);
    }

    void ApplyRudderForce(float rudderInput)
    {
        Vector3 flatForward = Vector3.ProjectOnPlane(Rigidbody.transform.forward, Vector3.up).normalized;
        Vector3 flatLeft = Vector3.ProjectOnPlane(-Rigidbody.transform.right, Vector3.up).normalized;
        float forwardSpeed = Vector3.Dot(Rigidbody.linearVelocity, flatForward);
        float rudderEffectiveness = m_rudderEffectiveness.Evaluate(forwardSpeed);
        Vector3 rudderForce = flatLeft * (rudderInput * rudderEffectiveness);
        Rigidbody.AddForceAtPosition(rudderForce * (m_rudderTurnMultiplier * 100), m_rudderPoint.position, ForceMode.Force);
        
    }
}
