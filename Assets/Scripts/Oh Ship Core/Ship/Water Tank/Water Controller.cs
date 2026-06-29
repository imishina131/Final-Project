using MatrixUtils.Attributes;
using MatrixUtils.AudioSystem;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class WaterController : MonoBehaviour
{
    [Inject] INotificationMessenger m_notificationMessenger;

    [Header("Required References")]
    [SerializeField, RequiredField] MeshRenderer m_waterFillMesh;

    [Header("Settings")]
    [SerializeField] float m_maxFill = 1f;
    [SerializeField] float m_minFill = -1f;
    [FormerlySerializedAs("m_fillChangeRate")] [SerializeField] float m_driftFillChangeRate = 0.1f;
    [SerializeField] float m_playerFillChangeRate = 0.1f;
    [SerializeField] float m_minHoldDuration = 120f;
    [SerializeField] float m_maxHoldDuration = 180f;
    [SerializeField] float m_warningThreshold = 0.2f;
    [SerializeField] float m_stableRange = 0.1f;
    [SerializeField] float m_failureTime = 10f;

    [Header("Events")]
    [SerializeField] UnityEvent OnUnderPressureThresholdReached;
    [SerializeField] UnityEvent OnUnderPressureMinimumReached;
    [SerializeField] UnityEvent OnOverPressureFailure;
    [SerializeField] UnityEvent OnOverPressureThresholdReached;
    [SerializeField] UnityEvent OnOverPressureMaximumReached;
    [SerializeField] UnityEvent OnUnderPressureFailure;
    [SerializeField] private UnityEvent<float> OnWaterFillUpdate =  new UnityEvent<float>();

    [Header("Sounds")]
    [SerializeField] SoundData m_overPressureThresholdReached;
    [SerializeField] SoundData m_overPressureMaximumReached;
    [SerializeField] SoundData m_overPressureFailure;
    [SerializeField] SoundData m_underPressureThresholdReached;
    [SerializeField] SoundData m_underPressureMinimumReached;
    [SerializeField] SoundData m_underPressureFailure;
    [SerializeField] SoundData m_pressureEqualized;
    
    float FillCenter => Mathf.Lerp(m_minFill, m_maxFill, 0.5f);
    public float CurrentFill { get; private set; }
    public float NormalizedFill => Mathf.InverseLerp(m_minFill, m_maxFill, CurrentFill);
    static readonly int s_fillProperty = Shader.PropertyToID("_Fill");
    float m_activeFillDirection;
    bool m_activeAlert;
    IDriftingEvent m_currentDrift;

    void Awake()
    {
        UpdateCurrentFill(FillCenter);
        ScheduleNextDrift();
    }

    void Update()
    {
        UpdateCurrentFill(CurrentFill + m_activeFillDirection * m_playerFillChangeRate * Time.deltaTime);
        OnWaterFillUpdate.Invoke(CurrentFill);
        m_currentDrift.OnEventUpdate(CurrentFill);
    }

    public void HandleFillInput(float fillDirection) => m_activeFillDirection = fillDirection;

    public void OnUserInteractionStarted() => m_currentDrift.OnEventPaused(CurrentFill);

    public void OnUserInteractionEnded()
    {
        m_activeFillDirection = 0f;
        m_currentDrift.OnEventResumed(CurrentFill);
    }
    
    void ScheduleNextDrift()
    {
        m_currentDrift = new NeutralDriftingEvent(StartDrift, m_minHoldDuration, m_maxHoldDuration);
        m_currentDrift.OnEventStart(CurrentFill);
    }

    void StartDrift()
    {
        float fillTarget = CurrentFill >= 0f ? m_maxFill : m_minFill;
        DriftEvent drift = new(
            UpdateCurrentFill,
            fill => Mathf.Abs(fill - FillCenter) <= m_stableRange,
            m_driftFillChangeRate,
            m_warningThreshold,
            fillTarget,
            m_failureTime
        );

        drift.OnWarningThresholdReached += fillTarget > 0f ? OverPressureWarning : UnderPressureWarning;
        drift.OnFailure += fillTarget > 0f ? OverPressureFailure : UnderPressureFailure;
        drift.OnFailureThresholdReached += fillTarget > 0f ? OverPressureMaximumReached : UnderPressureMinimumReached;
        drift.OnFailure += ScheduleNextDrift;
        drift.OnCancelled += OnPressureEqualized;
        drift.OnCancelled += ScheduleNextDrift;

        m_currentDrift = drift;
        m_currentDrift.OnEventStart(CurrentFill);
    }

    void UpdateCurrentFill(float newValue)
    {
        CurrentFill = Mathf.Clamp(newValue, m_minFill, m_maxFill);
        m_waterFillMesh.material.SetFloat(s_fillProperty, CurrentFill);
    }

    void OverPressureWarning()
    {
        if (m_activeAlert) return;
        m_activeAlert = true;
        m_notificationMessenger.TryNotify("enable steam");
        SoundManager.Instance.CreateSound().WithSoundData(m_overPressureThresholdReached).WithPosition(transform.position).WithRandomPitch().Play();
        OnOverPressureThresholdReached.Invoke();
    }

    void UnderPressureWarning()
    {
        if (m_activeAlert) return;
        m_activeAlert = true;
        m_notificationMessenger.TryNotify("enable steam");
        SoundManager.Instance.CreateSound().WithSoundData(m_underPressureThresholdReached).WithPosition(transform.position).WithRandomPitch().Play();
        OnUnderPressureThresholdReached.Invoke();
    }
    void OverPressureMaximumReached()
    {
        SoundManager.Instance.CreateSound().WithSoundData(m_overPressureMaximumReached).WithPosition(transform.position).WithRandomPitch().Play();
        OnOverPressureMaximumReached.Invoke();
    }

    void UnderPressureMinimumReached()
    {
        SoundManager.Instance.CreateSound().WithSoundData(m_underPressureMinimumReached).WithPosition(transform.position).WithRandomPitch().Play();
        OnUnderPressureMinimumReached.Invoke();
    }
    void OverPressureFailure()
    {
        OnOverPressureFailure.Invoke();
        SoundManager.Instance.CreateSound().WithSoundData(m_overPressureFailure).WithPosition(transform.position).WithRandomPitch().Play();
    }

    void UnderPressureFailure()
    {
        OnUnderPressureFailure.Invoke();
        SoundManager.Instance.CreateSound().WithSoundData(m_underPressureFailure).WithPosition(transform.position).WithRandomPitch().Play();
    }

    void OnPressureEqualized()
    {
        SoundManager.Instance.CreateSound().WithSoundData(m_pressureEqualized).WithPosition(transform.position).WithRandomPitch().Play();
        if (!m_activeAlert) return;
        m_activeAlert = false;
        m_notificationMessenger.TryNotify("disable steam");
    }
}