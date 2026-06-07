using UnityEngine;
/// <summary>
/// Represents a rigidbody that buoys itself upwards when it reaches a certain height.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BuoyantBody : MonoBehaviour
{
    [SerializeField] float m_waterHeight;
    [SerializeField] BuoyancyPoint[] m_buoyancyPoints;
    float m_buoyancyPercentage = 1;
    Rigidbody m_rigidbody;
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        foreach (BuoyancyPoint point in m_buoyancyPoints)
        {
            Vector3 globalPointPosition = transform.TransformPoint(point.LocalPosition);
            float depth = m_waterHeight - globalPointPosition.y;
            if (depth <= 0) continue;
            float submersion = Mathf.Clamp01(depth / point.Radius);
            m_rigidbody.AddForceAtPosition(Vector3.up * (submersion * point.PointBuoyancy * Physics.gravity.magnitude * m_rigidbody.mass * m_buoyancyPercentage), globalPointPosition);
            Vector3 pointVelocity = m_rigidbody.GetPointVelocity(globalPointPosition);
            Vector3 verticalVel = Vector3.Project(pointVelocity, Vector3.up);
            Vector3 longitudinalVel = Vector3.Project(pointVelocity, transform.forward);
            Vector3 lateralVel = pointVelocity - verticalVel - longitudinalVel;
            Vector3 drag = submersion * (verticalVel * point.Drag.y + longitudinalVel * point.Drag.x + lateralVel * point.Drag.z);
            m_rigidbody.AddForceAtPosition(-drag, globalPointPosition);
        }
    }

    public void UpdateSinkPercentage(float value) => m_buoyancyPercentage = Mathf.Lerp(1 , 0.7f, Mathf.Clamp01(value));
}