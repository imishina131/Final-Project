using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// Handle player movement through taking input from via <see cref="IPlayerControllable"/> from any <see cref="IPlayerController"/>
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(0f, 90f)] float maxGroundAngle = 25f;
    float minGroundDotProduct;
    bool onGround;

    Rigidbody m_rigidbody;
    Rigidbody boatRb;
    Vector2 m_desiredMovement;
    [SerializeField] float m_acceleration;
    [SerializeField] float m_deceleration;
    [SerializeField] float m_moveSpeed;
    [FormerlySerializedAs("cameraTurn")] [SerializeField] Transform m_camera;
    [FormerlySerializedAs("lookSens")] [SerializeField] float m_lookSensitivity = 30f;
    float m_lookPitch;
    Quaternion m_lookYaw = Quaternion.identity;
    Vector2 m_currentLookInput;
    IPlayerController m_playerController;
    public void OnMovementInputChanged(Vector2 input) => m_desiredMovement = Vector2.ClampMagnitude(input, 1) * m_moveSpeed;
    public void OnLookInputChanged(Vector2 input) => m_currentLookInput = input;
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        GameObject boatObj = GameObject.FindWithTag("Boat");
        boatRb = boatObj.GetComponent<Rigidbody>();

        FixedJoint joint = boatObj.AddComponent<FixedJoint>();

        joint.connectedBody = m_rigidbody;
    }

    void Awake()
    {
        OnValidate();
    }

    void FixedUpdate()
    {
        Vector3 flattenedVelocity = new(m_rigidbody.linearVelocity.x, 0, m_rigidbody.linearVelocity.z);
        float forwardVelocity  = Vector3.Dot(m_lookYaw * Vector3.forward, flattenedVelocity);
        float sidewaysVelocity = Vector3.Dot(m_lookYaw * Vector3.right,   flattenedVelocity);
        float newForward  = Mathf.MoveTowards(forwardVelocity,  m_desiredMovement.y, GetRate(forwardVelocity,  m_desiredMovement.y) * Time.fixedDeltaTime);
        float newSideways = Mathf.MoveTowards(sidewaysVelocity, m_desiredMovement.x, GetRate(sidewaysVelocity, m_desiredMovement.x) * Time.fixedDeltaTime);
        Vector3 worldVelocity = m_lookYaw * new Vector3(newSideways, 0, newForward);
        m_rigidbody.linearVelocity = new(worldVelocity.x, m_rigidbody.linearVelocity.y, worldVelocity.z);
    }

    void LateUpdate()
    {
        m_lookYaw *= Quaternion.Euler(0, m_currentLookInput.x * m_lookSensitivity * Time.deltaTime, 0);
        m_lookPitch = Mathf.Clamp(m_lookPitch - m_currentLookInput.y * m_lookSensitivity * Time.deltaTime, -90, 90);
        m_camera.rotation = m_lookYaw * Quaternion.Euler(m_lookPitch, 0, 0);
    }
    float GetRate(float current, float desired) => Mathf.Abs(current) < Mathf.Abs(desired) || !Mathf.Approximately(Mathf.Sign(current), Mathf.Sign(desired)) ? m_acceleration : m_deceleration;

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    void OnCollisionStay()
    {
        onGround = true;
    }

    void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            onGround |= normal.y >= minGroundDotProduct;
        }
    }
}
