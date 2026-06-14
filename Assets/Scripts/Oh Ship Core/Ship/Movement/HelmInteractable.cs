using MatrixUtils.Attributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
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
    IPlayerController m_activePlayerController;
    Vector2 m_moveInput = Vector2.zero;
    Vector2 m_lookInput = Vector2.zero;
    InteractionSession m_currentInteractionSession;
    ///<inheritdoc/>
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        if(interactor.IsInteracting() || m_currentInteractionSession is { IsActive: true }) return null;   
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
        m_shipMovement.SetThrottle(m_shipMovement.Throttle + m_moveInput.y * Time.deltaTime * m_helmThrottleSpeed);
        m_helmCamera.transform.Rotate(0, m_lookInput.x * Time.deltaTime * 100, 0);
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

    public Vector3 GetWidgetWorldPosition() => transform.position;
}
