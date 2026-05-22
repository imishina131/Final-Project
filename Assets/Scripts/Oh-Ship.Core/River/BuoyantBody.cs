using UnityEngine;
/// <summary>
/// Represents a rigidbody that buoys itself upwards when it reaches a certain height.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BuoyantBody : MonoBehaviour
{
    [SerializeField] float m_waterHeight;
    Rigidbody m_rigidbody;
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        if(m_rigidbody.position.y >= m_waterHeight) return;
        m_rigidbody.AddForce((Mathf.Abs(m_rigidbody.position.y - m_waterHeight) + 9.8f) * Vector3.up);
    }
}
