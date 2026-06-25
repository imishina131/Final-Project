using MatrixUtils.Attributes;
using MatrixUtils.AudioSystem;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls the fill of the water tank using states that use the <see cref="IFillState"/> interface.
/// </summary>
public class WaterController : MonoBehaviour
{
    [Inject] INotificationMessenger m_notificationMessenger;
    [Header("Required References")]
    [SerializeField, RequiredField] MeshRenderer m_waterFillMesh;
    [Header("Settings")]
    [SerializeField] float m_maxFill = 1f;
    [SerializeField] float m_minFill = -1f;
    [SerializeField] float m_fillChangeRate = 0.1f;
    [SerializeField] float m_minHoldDuration = 120f;
    [SerializeField] float m_maxHoldDuration = 180f;
    [SerializeField] float m_warningThreshold = 0.2f;
    [SerializeField] float m_failureTime = 10f;
    [Header("Events")]
    [SerializeField] UnityEvent OnUnderPressureThresholdReached;
    [SerializeField] UnityEvent OnOverPressureFailure;
    [SerializeField] UnityEvent OnOverPressureThresholdReached;
    [SerializeField] UnityEvent OnUnderPressureFailure;
    [Header("Sounds")] 
    [SerializeField] SoundData m_overPressureThresholdReached;
    [SerializeField] SoundData m_overPressureFailure;
    [SerializeField] SoundData m_underPressureThresholdReached;
    [SerializeField] SoundData m_underPressureFailure;
    [SerializeField] SoundData m_pressureEqualized;
    
    public float CurrentFill { get; private set; }
    static readonly int s_fillProperty = Shader.PropertyToID("_Fill");
    IFillState m_activeFillChange;
    NeutralFillState m_neutral;
    ChangeFillState m_increase;
    ChangeFillState m_decrease;
    bool m_activeAlert;
    
    /// <summary>
    /// Attempts to increase the fill of the water tank.
    /// </summary>
    public void IncreaseWaterFill() => m_activeFillChange.HandleIncrease();
    /// <summary>
    /// Attempts to decrease the fill of the water tank.
    /// </summary>
    public void DecreaseWaterFill() => m_activeFillChange.HandleDecrease();
    
    bool m_hasStartedWarning;
    void Awake()
    {
        //I am not a fan of all the state configuration being here, but it works. I suppose it could be done in the states themselves or via a configuration class, but I don't really care right now
        m_neutral = new(UpdateWaterFillChange, m_fillChangeRate, m_minHoldDuration, m_maxHoldDuration);
        m_increase = new(UpdateWaterFillChange, m_maxFill, m_fillChangeRate, m_warningThreshold, m_failureTime);
        m_decrease = new(UpdateWaterFillChange, m_minFill, m_fillChangeRate, m_warningThreshold, m_failureTime);
        
        m_neutral.OnIncrease = m_increase;
        m_neutral.OnDecrease = m_decrease;
        
        m_increase.OnIncrease = m_increase;
        m_increase.OnDecrease = m_neutral;
        m_increase.OnWarningTriggered += OverPressureWarning;
        m_increase.OnFailureTriggered += OverPressureFailure;
        
        m_decrease.OnIncrease = m_neutral;
        m_decrease.OnDecrease = m_decrease;
        m_decrease.OnWarningTriggered += UnderPressureWarning;
        m_decrease.OnFailureTriggered += UnderPressureFailure;
        
        UpdateWaterFillChange(m_neutral);
        m_neutral.OnEnteredNeutral += OnPressureEqualized;
        
        FindAnyObjectByType<Injector>().Inject(this);
    }

    void UpdateWaterFillChange(IFillState fillChange)
    {
        if (m_activeFillChange != null)
        {
            m_activeFillChange.OnFillChange -= UpdateCurrentFill;
            m_activeFillChange.OnEventEnded();
        }
        m_activeFillChange = fillChange;
        m_activeFillChange.OnFillChange += UpdateCurrentFill;
        m_activeFillChange.OnEventStarted(CurrentFill);
    }

    void UpdateCurrentFill(float newValue)
    {
        CurrentFill = Mathf.Clamp(newValue, m_minFill, m_maxFill);
        m_waterFillMesh.material.SetFloat(s_fillProperty, CurrentFill);
    }

    void OverPressureWarning()
    {
        if(m_activeAlert) return;
        m_activeAlert = true;
        m_notificationMessenger.TryNotify("enable steam");
        SoundManager.Instance.CreateSound().WithSoundData(m_overPressureThresholdReached).WithPosition(transform.position).WithRandomPitch().Play();
        OnOverPressureThresholdReached.Invoke();
    }
    
    void UnderPressureWarning()
    {
        if(m_activeAlert) return;
        m_activeAlert = true;
        m_notificationMessenger.TryNotify("enable steam");
        SoundManager.Instance.CreateSound().WithSoundData(m_underPressureThresholdReached).WithPosition(transform.position).WithRandomPitch().Play();
        OnUnderPressureThresholdReached.Invoke();
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