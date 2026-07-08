using MatrixUtils.Attributes;
using MatrixUtils.DependencyInjection;
using MatrixUtils.Extensions;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Windows;

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
    [SerializeField] ProcedurallyAnimatedElement m_valveElement;
    PlayerInteractionState m_currentInteractionState;
    InteractionSession m_currentInteractionSession;
    GameObject m_player;

    private AudioSource audio;

    [Header ("Audio")]
    [SerializeField] private AudioClip wheelTurning;
    
    /// <inheritdoc/>
    /// 
    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        IPlayerControllable oldControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        IPlayerController controller = oldControllable.GetActivePlayerController();
        m_currentInteractionState = oldControllable.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();

        if(m_currentInteractionState.CheckInteractionTag(InteractionTag.HoldingFish) || m_currentInteractionState.CheckInteractionTag(InteractionTag.HoldingBottle) || m_currentInteractionState.CheckInteractionTag(InteractionTag.HoldingBottleWithWater) || m_currentInteractionSession is {IsActive: true})
        {
            Debug.Log("Blocked interaction");
            return null;
        }
        CinemachineCamera playerCamera = interactor.GetAssociatedGameObject().GetComponent<CinemachineCamera>();
        m_steamPressureCamera.OutputChannel =  playerCamera.OutputChannel;
        m_steamPressureCamera.Priority = 10;
        m_currentInteractionState.AddInteractionTag(InteractionTag.AdjustingWaterTank);
        controller.ChangeControlledEntity(this);
        m_player = oldControllable.GetAssociatedGameObject();
        m_player.GetComponentInChildren<MeshRenderer>().enabled = false;
        
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

        InputAction adjustPressureAction = map.FindAction("Adjust Pressure");
        adjustPressureAction.performed += HandleAdjustPressure;
        adjustPressureAction.canceled += HandleAdjustPressure;
        InputAction interactAction = map.FindAction("Interact");
        interactAction.performed += HandleInteract;
        m_pressureSystem.OnUserInteractionStarted();
    }

    public void OnControlReleased()
    {
        if (m_activePlayerController == null) throw new("Player controller is null, cannot release control.");
        if (!m_activePlayerController.TryGetCurrentInputActionMap(out InputActionMap map)) throw new("Player controller is not null, but input action map is null...");
        m_steamPressureCamera.Priority = 0;

        InputAction adjustPressureAction = map.FindAction("Adjust Pressure");
        adjustPressureAction.performed -= HandleAdjustPressure;
        adjustPressureAction.canceled -= HandleAdjustPressure;
        InputAction interactAction = map.FindAction("Interact");
        interactAction.performed -= HandleInteract;
        m_player.GetComponentInChildren<MeshRenderer>().enabled = true;
        m_currentInteractionState.RemoveInteractionTag(InteractionTag.AdjustingWaterTank);
        m_currentInteractionSession.End();
        m_activePlayerController = null;
        m_pressureSystem.HandleFillInput(0);
        m_pressureSystem.OnUserInteractionEnded();
    }
    void HandleAdjustPressure(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<float>();
        m_pressureSystem.HandleFillInput(context.ReadValue<float>());

        if (input != 0)
        {
            if (!audio.isPlaying)
            {
                audio.clip = wheelTurning;
                audio.loop = true;
                audio.Play();
            }
        }
        else
        {
            audio.Pause();
        }
    }
    void Update()
    {
        m_valveElement.Transform.localEulerAngles = new(m_valveElement.GetNextAngle(m_pressureSystem.NormalizedFill, m_valveElement.Transform.localEulerAngles.x),0,0);
    }

    /// <inheritdoc/>
    public IPlayerController GetActivePlayerController() => m_activePlayerController;
    void HandleInteract(InputAction.CallbackContext context) => m_currentInteractionSession.End();
    public GameObject GetAssociatedGameObject() => gameObject;
    public PromptData GetPromptData() => new() {AssociatedWidget = m_widgetForPrompt};
    public Vector3 GetWidgetWorldPosition() => m_displayForInteraction.position;
    
    [Serializable]
    class ProcedurallyAnimatedElement
    {
        public Transform Transform;
        public float MinAngle;
        public float MaxAngle;
        float m_velocity;

        public float GetNextAngle(float normalizedDesiredAngle, float currentAngle)
        {
            float desiredWheelAngle = Mathf.Lerp(MinAngle, MaxAngle, normalizedDesiredAngle);
            return Mathf.SmoothDampAngle(currentAngle, desiredWheelAngle, ref m_velocity, 0.1f);
        }
    }
}
