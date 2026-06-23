using System;
using MatrixUtils.Attributes;
using MatrixUtils.Timers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
public class ShipHole : MonoBehaviour, IInteractable, IPromptProvider, IProgressRepairProvider
{
    [FormerlySerializedAs("_widgetForPrompt")] [SerializeField]
    string m_widgetForPrompt = "interact";
    [FormerlySerializedAs("_widgetForRepair")] [SerializeField]
    string m_widgetForRepair = "repair";
    [SerializeField] float m_repairTime;
    [FormerlySerializedAs("m_leakEffect")] public VisualEffect LeakEffect;
    [SerializeField, RequiredField] AudioSource m_repairAudioSource;
    [SerializeField, RequiredField] AudioSource m_leakAudioSource;
    CountdownTimer m_repairTimer;
    InteractionSession m_currentInteraction;
    bool m_isRepairing;
    Action<float> m_onRepairProgressCheck;
    Action m_onRepairComplete;
    PromptDisplay m_promptDisplay;
    IPlayerControllable m_controllable;
    IPlayerController m_playerController;

    
    public void Initialize(Action onRepairComplete)
    {
        m_onRepairComplete = onRepairComplete;
        m_repairTimer = new(m_repairTime);
        m_repairTimer.OnTimerStop += HandleRepairEnd;
        m_repairTimer.OnTimerTick += () => m_onRepairProgressCheck?.Invoke(1- m_repairTimer.Progress);
        m_leakAudioSource.Play();
    }
    /// <inheritdoc/>
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        if (m_currentInteraction is {IsActive: false}) return null;
        m_controllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        m_playerController = m_controllable.GetActivePlayerController();
        m_promptDisplay = m_playerController.GetAssociatedGameObject().GetComponentInChildren<PromptDisplay>();
        InteractionSession session = new(interactor, this);
        m_onRepairProgressCheck?.Invoke(1 - m_repairTimer.Progress);
        m_currentInteraction = session;
        m_promptDisplay.HidePrompt(this);
        m_isRepairing = true;
        m_promptDisplay.ShowPrompt(this);
        m_repairAudioSource.Play();
        session.OnEnded += () =>
        {
            m_currentInteraction = null;
            if (m_repairTimer is null) return;
            m_promptDisplay.HidePrompt(this);
            m_isRepairing = false;
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
            m_isRepairing = false;
            m_repairTimer = null;
            m_repairAudioSource.Stop();
            m_leakAudioSource.Stop();
            return;
        }
        m_repairAudioSource.Pause();
        m_repairTimer.Pause();
    }

    public PromptData GetPromptData() => new() {AssociatedWidget =  m_isRepairing? m_widgetForRepair : m_widgetForPrompt };
    public Vector3 GetWidgetWorldPosition() => transform.position;
    public void AddProgressSubscriber(Action<float> sub) => m_onRepairProgressCheck += sub;
    public void RemoveProgressSubscriber(Action<float> sub) => m_onRepairProgressCheck -= sub;
}
