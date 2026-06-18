using MatrixUtils.Attributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// An interactable that allows the player to adjust the pressure of <see cref="WaterController"/> on the ship.
/// </summary>
public class WaterValveInteractable : MonoBehaviour, IInteractable, IPlayerControllable, IPromptProvider
{
    [FormerlySerializedAs("_displayForInteraction")]
    [SerializeField] Transform m_displayForInteraction;
    [FormerlySerializedAs("_playerInteractionLocation")] 
    [SerializeField, RequiredField] Transform m_playerInteractionLocation;
    [FormerlySerializedAs("_steamPressureCamera")] [SerializeField]
    CinemachineCamera m_steamPressureCamera;
    [SerializeField] string m_widgetForPrompt = "interact";
    IPlayerController m_activePlayerController;
    [SerializeField] string m_pressureControlActionMap = "Adjust Pressure";
    [SerializeField, RequiredField] WaterController m_pressureSystem;
    
    InteractionSession m_currentInteractionSession;

    private GameObject player;
    /// <inheritdoc/>
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        IPlayerControllable oldControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        IPlayerController controller = oldControllable.GetActivePlayerController();
        controller.ChangeControlledEntity(this);
       
        CinemachineCamera playerCamera = interactor.GetAssociatedGameObject().GetComponent<CinemachineCamera>();
        
        m_steamPressureCamera.OutputChannel =  playerCamera.OutputChannel;
        m_steamPressureCamera.Priority = 10;

        player = oldControllable.GetAssociatedGameObject();
       
        player.GetComponentInChildren<MeshRenderer>().enabled = false;
        
        m_currentInteractionSession = new(interactor, this);
        m_currentInteractionSession.OnEnded += () => controller.ChangeControlledEntity(oldControllable);
        return m_currentInteractionSession;
    }
    /// <inheritdoc/>
    public void OnControlRequested(IPlayerController player)
    {
        m_activePlayerController = player;
        if (!player.TryChangeInputActionMap(m_pressureControlActionMap, out InputActionMap map))
        {
            Debug.LogError("Failed to assign input actions to player, reverting control to default.");
            player.ChangeControlledEntity(null);
            return;
        }
        
        InputAction increasePressureAction = map.FindAction("Increase Pressure");
        increasePressureAction.performed += HandleIncreasePressure;
        InputAction decreasePressureAction = map.FindAction("Decrease Pressure");
        decreasePressureAction.performed += HandleDecreasePressure;
        InputAction interactAction = map.FindAction("Interact");
        interactAction.performed += HandleInteract;
    }
    /// <inheritdoc/>
    public void OnControlReleased()
    {
        if (m_activePlayerController == null) throw new("Player controller is null, cannot release control.");
        if (!m_activePlayerController.TryGetCurrentInputActionMap(out InputActionMap map)) throw new("Player controller is not null, but input action map is null...");
        m_steamPressureCamera.Priority = 0;
        
        InputAction increasePressureAction = map.FindAction("Increase Pressure");
        increasePressureAction.performed -= HandleIncreasePressure;
        InputAction decreasePressureAction = map.FindAction("Decrease Pressure");
        decreasePressureAction.performed -= HandleDecreasePressure;
        InputAction interactAction = map.FindAction("Interact");
        interactAction.performed -= HandleInteract;
        player.GetComponentInChildren<MeshRenderer>().enabled = true;
        m_activePlayerController = null;
    }
    
    /// <inheritdoc/>
    public IPlayerController GetActivePlayerController() => m_activePlayerController;
    void HandleIncreasePressure(InputAction.CallbackContext context) => m_pressureSystem.IncreaseWaterFill();
    void HandleDecreasePressure(InputAction.CallbackContext context) => m_pressureSystem.DecreaseWaterFill();
    void HandleInteract(InputAction.CallbackContext context) => m_currentInteractionSession.End();
    public GameObject GetAssociatedGameObject() => gameObject;
    public PromptData GetPromptData() => new() {AssociatedWidget = m_widgetForPrompt};
    public Vector3 GetWidgetWorldPosition() => m_displayForInteraction.position;
}
