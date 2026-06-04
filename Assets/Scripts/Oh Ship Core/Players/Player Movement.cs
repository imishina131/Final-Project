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

    Rigidbody connectedBody, pastConnectedBody;
    int groundContactCount;

    float pastPlatformYaw, platformYaw;

    Vector3 contactNormal, connectionWorldPos, connectionLocalPos;
    Vector3 velocity, connectionVelocity;

    Rigidbody m_rigidbody;
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
    }

    void Awake()
    {

    }

    void FixedUpdate()
    {
        UpdateState();
        AdjustVelocityAndRotation();


        ClearState();
    }

    void LateUpdate()
    {
        m_lookYaw *= Quaternion.Euler(0, m_currentLookInput.x * m_lookSensitivity * Time.deltaTime, 0);
        m_lookPitch = Mathf.Clamp(m_lookPitch - m_currentLookInput.y * m_lookSensitivity * Time.deltaTime, -90, 90);
        m_camera.rotation = m_lookYaw * Quaternion.Euler(m_lookPitch, 0, 0);
    }
    float GetRate(float current, float desired) => Mathf.Abs(current) < Mathf.Abs(desired) || !Mathf.Approximately(Mathf.Sign(current), Mathf.Sign(desired)) ? m_acceleration : m_deceleration;

    void Update()
    {

    }


    void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;

            groundContactCount += 1;
            contactNormal += normal;
            if(!connectedBody)
            {
                connectedBody = collision.rigidbody;
            }
        }

        if (groundContactCount > 1)
        {
            contactNormal.Normalize();
        }
    }

    void ClearState()
    {
        groundContactCount = 0;
        contactNormal = connectionVelocity = Vector3.zero;
        pastConnectedBody = connectedBody;
        connectedBody = null;
    }

    void UpdateState()
    {
        velocity = m_rigidbody.linearVelocity;
        if (connectedBody)
        {
            if (connectedBody.isKinematic || connectedBody.mass >= m_rigidbody.mass)
            {
                UpdateConnectionState();
            }
        }
    }

    void UpdateConnectionState()
    {
        if (connectedBody == pastConnectedBody)
        {
            Vector3 currentWorldPos = connectedBody.transform.TransformPoint(connectionLocalPos);
            Vector3 connectionMovement = currentWorldPos - connectionWorldPos;
            connectionVelocity = connectionMovement / Time.fixedDeltaTime;

            platformYaw = connectedBody.transform.eulerAngles.y;

            float yawDelta = Mathf.DeltaAngle(pastPlatformYaw, platformYaw);
            m_lookYaw = Quaternion.Euler(0f, yawDelta, 0f) * m_lookYaw;
        }
        connectionWorldPos = m_rigidbody.position;
        connectionLocalPos = connectedBody.transform.InverseTransformPoint(connectionWorldPos);
        pastPlatformYaw = platformYaw;
    }

    void AdjustVelocityAndRotation()
    {
        if(connectedBody)
        {
            Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
            Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

            float connectionVelocityX = Vector3.Dot(m_lookYaw * Vector3.forward, connectionVelocity);
            float connectionVelocityZ = Vector3.Dot(m_lookYaw * Vector3.right, connectionVelocity);
            float currentX = Vector3.Dot(m_lookYaw * Vector3.forward, xAxis);
            float currentZ = Vector3.Dot(m_lookYaw * Vector3.right, zAxis);


            float yawDelta = Mathf.DeltaAngle(pastPlatformYaw, platformYaw);
            Quaternion platformRotation = Quaternion.Euler(0f, yawDelta, 0f);

            Vector3 offset = m_rigidbody.position - connectedBody.position;
            Vector3 rotatedOffset = platformRotation * offset;
            Vector3 rotationVelocity = (rotatedOffset - offset) / Time.fixedDeltaTime;

            float rotationVelocityX = Vector3.Dot(m_lookYaw * Vector3.forward, rotationVelocity);
            float rotationVelocityZ = Vector3.Dot(m_lookYaw * Vector3.right, rotationVelocity);


            float forwardVelocity = currentX - connectionVelocityX;
            float sidewaysVelocity = currentZ - connectionVelocityZ;
            float newForward = Mathf.MoveTowards(forwardVelocity, m_desiredMovement.y, GetRate(forwardVelocity, m_desiredMovement.y)) + connectionVelocityX;
            float newSideways = Mathf.MoveTowards(sidewaysVelocity, m_desiredMovement.x, GetRate(sidewaysVelocity, m_desiredMovement.x)) + connectionVelocityZ;
            Vector3 worldVelocity = m_lookYaw * new Vector3(newSideways, 0, newForward);
            m_rigidbody.linearVelocity = new(worldVelocity.x, m_rigidbody.linearVelocity.y, worldVelocity.z);
        }

    }


    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

}
