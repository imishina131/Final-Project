using System;
using MatrixUtils.Attributes;
using MatrixUtils.Extensions;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class HelmInteractable : MonoBehaviour, IInteractable, IPlayerControllable, IPromptProvider
{
    [FormerlySerializedAs("_widgetForPrompt")] [SerializeField] string m_widgetForPrompt = "interact";
    [SerializeField] string m_helmControlActionMap = "Helm";
    [SerializeField] CinemachineCamera m_helmCamera;
    [SerializeField] float m_helmThrottleSpeed = 0.5f;
    [SerializeField] float m_helmRudderSpeed = 0.5f;
    [SerializeField, RequiredField] ShipMovement m_shipMovement;
    [FormerlySerializedAs("_interactDisplayTransform")] [SerializeField] Transform m_interactDisplayTransform;
    [SerializeField] ProcedurallyAnimatedHelmElement m_wheelElement;
    [SerializeField] ProcedurallyAnimatedHelmElement m_throttleElement;
    [SerializeField] ProcedurallyAnimatedHelmElement m_speedometerElement;
    IPlayerController m_activePlayerController;
    Vector2 m_moveInput = Vector2.zero;
    Vector2 m_lookInput = Vector2.zero;
    InteractionSession m_currentInteractionSession;
    float m_wheelVelocity;
    ///<inheritdoc/>
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        Debug.Log($"WasInteracting: {interactor.WasInteracting}, Session active: {m_currentInteractionSession?.IsActive}");
        if (interactor.WasInteracting || m_currentInteractionSession is { IsActive: true })
        {
            Debug.Log("Blocking helm interaction");
            return null;
        }   
        IPlayerControllable oldControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        
        CinemachineCamera playerCam = interactor.GetAssociatedGameObject().GetComponent<CinemachineCamera>();
        m_helmCamera.OutputChannel = playerCam.OutputChannel;
        m_helmCamera.Priority = 10;
        IPlayerController controller = oldControllable.GetActivePlayerController();
      
        controller.ChangeControlledEntity(this);
        m_currentInteractionSession = new(interactor, this);
        m_currentInteractionSession.OnEnded += () => controller.ChangeControlledEntity(oldControllable); 
        return m_currentInteractionSession;
    }

    void Update()
    {
        if(m_activePlayerController is null) return;
        m_shipMovement.SetRudder(m_shipMovement.Rudder + m_moveInput.x * Time.deltaTime * m_helmRudderSpeed);
        m_wheelElement.Transform.localEulerAngles = new(0, 0, m_wheelElement.GetNextAngle(m_shipMovement.Rudder, m_wheelElement.Transform.localEulerAngles.z));
        m_shipMovement.SetThrottle(m_shipMovement.Throttle + m_moveInput.y * Time.deltaTime * m_helmThrottleSpeed);
        m_throttleElement.Transform.localEulerAngles = new(m_throttleElement.GetNextAngle(-m_shipMovement.Throttle, m_throttleElement.Transform.localEulerAngles.x), 0, 0);
        m_speedometerElement.Transform.localEulerAngles = new(0, 0, m_speedometerElement.GetNextAngle(Vector3.Dot(m_shipMovement.Rigidbody.linearVelocity, m_shipMovement.Rigidbody.transform.forward).Remap(-14, 14, -1, 1), m_speedometerElement.Transform.localEulerAngles.z));        m_helmCamera.transform.Rotate(0, m_lookInput.x * Time.deltaTime * 100, 0);
    }
    ///<inheritdoc/>
    public void OnControlRequested(IPlayerController player)
    {
        m_activePlayerController = player;
        if (!player.TryChangeInputActionMap(m_helmControlActionMap, out InputActionMap map))
        {
            Debug.LogError("Failed to assign input actions to player, reverting control to default.");
            player.ChangeControlledEntity(null);
            return;
        }
        InputAction movementAction = map.FindAction("Move");
        movementAction.performed += HandleMovementInput;
        movementAction.canceled += HandleMovementInput;
        InputAction interactAction = map.FindAction("Interact");
        interactAction.performed += HandleInteract;
        InputAction lookAction = map.FindAction("Look");
        lookAction.performed += HandleLookInput;
        lookAction.canceled += HandleLookInput;
        
    }
    ///<inheritdoc/>
    public void OnControlReleased()
    {
        m_moveInput = Vector2.zero;
        m_helmCamera.Priority = 0;
        Update();
        if (!m_activePlayerController.TryGetCurrentInputActionMap(out InputActionMap map)) return;
        InputAction movementAction = map.FindAction("Move");
        movementAction.performed -= HandleMovementInput;
        movementAction.canceled -= HandleMovementInput;
        InputAction interactAction = map.FindAction("Interact");
        interactAction.performed -= HandleInteract;
        InputAction lookAction = map.FindAction("Look");
        lookAction.performed -= HandleLookInput;
        lookAction.canceled -= HandleLookInput;
        m_activePlayerController = null;
    }
    ///<inheritdoc/>
    public IPlayerController GetActivePlayerController() => m_activePlayerController;

    void HandleMovementInput(InputAction.CallbackContext context) => m_moveInput = context.ReadValue<Vector2>();
    void HandleInteract(InputAction.CallbackContext context) => m_currentInteractionSession.End();
    void HandleLookInput(InputAction.CallbackContext context) => m_lookInput = context.ReadValue<Vector2>();

    public GameObject GetAssociatedGameObject() => gameObject;

    public PromptData GetPromptData() => new() {AssociatedWidget = m_widgetForPrompt};

    public Vector3 GetWidgetWorldPosition() => m_interactDisplayTransform.position;
    
    [Serializable]
    class ProcedurallyAnimatedHelmElement
    {
        public Transform Transform;
        public float MinAngle;
        public float MaxAngle;
        float m_velocity;

        public float GetNextAngle(float normalizedDesiredAngle, float currentAngle)
        {
            float desiredWheelAngle = Mathf.Lerp(MinAngle, MaxAngle, normalizedDesiredAngle.Remap(-1, 1, 1, 0));
            return Mathf.SmoothDampAngle(currentAngle, desiredWheelAngle, ref m_velocity, 0.1f);
        }
    }
}
