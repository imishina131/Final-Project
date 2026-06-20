using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControlRouter : MonoBehaviour, IPlayerControllable
{
    [SerializeField] string m_requiredInputActionMap;
    [SerializeField] UnityEvent<Vector2> m_onMovementInputChanged;
    [SerializeField] UnityEvent<Vector2> m_onLookInputChanged;
    [SerializeField] UnityEvent m_onInteract;
    [SerializeField] UnityEvent m_onUse;
    [SerializeField] UnityEvent<IPlayerController> m_onPlayerControlStarted;
    [SerializeField] UnityEvent<IPlayerController> m_onPlayerControlEnded;
    IPlayerController m_playerController;
    InputActionMap m_activeActionMap;
    void OnMovementInputChanged(InputAction.CallbackContext context) => m_onMovementInputChanged.Invoke(context.ReadValue<Vector2>());
    void OnLookInputChanged(InputAction.CallbackContext context) => m_onLookInputChanged.Invoke(context.ReadValue<Vector2>());
    void OnInteract(InputAction.CallbackContext context) => m_onInteract.Invoke();
    void OnUse(InputAction.CallbackContext context) => m_onUse.Invoke();
    
    public void OnControlRequested(IPlayerController player)
    {
        if (!player.TryChangeInputActionMap(m_requiredInputActionMap, out InputActionMap map))
        {
            Debug.LogError("Failed to assign input actions to player, reverting control to default.");
            player.ChangeControlledEntity(null);
            return;
        }
        
        m_activeActionMap = map;
        m_playerController = player;

        InputAction movementAction = m_activeActionMap.FindAction("Move");
        movementAction.performed += OnMovementInputChanged;
        movementAction.canceled += OnMovementInputChanged;

        InputAction lookAction = m_activeActionMap.FindAction("Look");
        lookAction.performed += OnLookInputChanged;
        lookAction.canceled += OnLookInputChanged;
        
        InputAction interactAction = m_activeActionMap.FindAction("Interact");
        interactAction.performed += OnInteract;
        m_onPlayerControlStarted.Invoke(player);
        
        InputAction useAction = m_activeActionMap.FindAction("Use");
        useAction.performed += OnUse;
    }
    /// <inheritdoc/>
    public void OnControlReleased()
    {
        InputAction movementAction = m_activeActionMap.FindAction("Move");
        movementAction.performed -= OnMovementInputChanged;
        movementAction.canceled -= OnMovementInputChanged;
        m_onMovementInputChanged.Invoke(Vector2.zero);
        InputAction lookAction = m_activeActionMap.FindAction("Look");
        lookAction.performed -= OnLookInputChanged;
        lookAction.canceled -= OnLookInputChanged;
        m_onLookInputChanged.Invoke(Vector2.zero);
        InputAction interactAction = m_activeActionMap.FindAction("Interact");
        interactAction.performed -= OnInteract;
        InputAction useAction = m_activeActionMap.FindAction("Use");
        useAction.performed -= OnUse;
        m_playerController = null;
        m_activeActionMap = null;
        m_onPlayerControlEnded.Invoke(m_playerController);
        
    }
    /// <inheritdoc/>
    [Pure, CanBeNull] public IPlayerController GetActivePlayerController() => m_playerController;

    public GameObject GetAssociatedGameObject() => gameObject;
}
