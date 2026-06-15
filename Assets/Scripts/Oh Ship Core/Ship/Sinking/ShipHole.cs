using System;
using MatrixUtils.Timers;
using UnityEngine;
using UnityEngine.Events;

public class ShipHole : MonoBehaviour, IInteractable, IPromptProvider, IProgressRepairProvider
{
    [SerializeField] private string _widgetForPrompt = "interact";
    [SerializeField] private string _widgetForRepair = "repair";
    Action m_onRepairComplete;
    [SerializeField] float m_repairTime;
    CountdownTimer m_repairTimer;
    InteractionSession m_currentInteraction;

    private bool _isRepairng = false;
    public Action<float> onRepairProgressCheck;
    
    private PromptDisplay m_promptDisplay;
    private IPlayerControllable m_controllable;
    private IPlayerController m_playerController;

    public void Initialize(Action onRepairComplete)
    {
        m_onRepairComplete = onRepairComplete;
        m_repairTimer = new(m_repairTime);
        m_repairTimer.OnTimerStop += HandleRepairEnd;
        m_repairTimer.OnTimerTick += () => onRepairProgressCheck?.Invoke(1- m_repairTimer.Progress);
    }
    /// <inheritdoc/>
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        if (m_currentInteraction is {IsActive: false}) return null;
      
        m_controllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        m_playerController = m_controllable.GetActivePlayerController();
        
        m_promptDisplay = m_playerController.GetAssociatedGameObject().GetComponentInChildren<PromptDisplay>();
        
        InteractionSession session = new(interactor, this);
        onRepairProgressCheck?.Invoke(1 - m_repairTimer.Progress);
        m_currentInteraction = session;
        m_promptDisplay.HidePrompt(this);
        _isRepairng = true;
        m_promptDisplay.ShowPrompt(this);
        session.OnEnded += () =>
        {
            m_currentInteraction = null;
            if (m_repairTimer is null) return;
            m_promptDisplay.HidePrompt(this);
            _isRepairng = false;
            m_promptDisplay.ShowPrompt(this);
            m_repairTimer.Pause();
        };
        
        m_repairTimer.Start();
        
        return session;
    }

    void HandleRepairEnd()
    {
        if (m_repairTimer is null) return;
        if (m_repairTimer.IsFinished)
        {
            m_onRepairComplete?.Invoke();
            _isRepairng = false;
            m_repairTimer = null;
            return;
        }
        
        m_repairTimer.Pause();
    }

    public PromptData GetPromptData()
    {
        return new PromptData{AssociatedWidget =  _isRepairng? _widgetForRepair : _widgetForPrompt };
    }

    public Vector3 GetWidgetWorldPosition()
    {
        return transform.position;
    }

    public void AddProgressSubscriber(Action<float> sub) => onRepairProgressCheck += sub;
    public void RemoveProgressSubscriber(Action<float> sub) => onRepairProgressCheck -= sub;
}
