using System;
using MatrixUtils.Timers;
using UnityEngine;
using Random = UnityEngine.Random;
/// <summary>
/// The neutral fill state of the water tank where the tank is not filling or draining and is moving towards the neutral fill position
/// </summary>
public class NeutralFillState : IFillState
{
    const float TargetFill = 0;
    readonly Action<IFillState> m_requestTransitionAction;
    readonly float m_fillRate;
    public IFillState OnIncrease;
    public IFillState OnDecrease;
    readonly float m_minHoldDuration;
    readonly float m_maxHoldDuration;

    float m_startingFill;
    CountdownTimer m_driftTimer;
    CountdownTimer m_holdTimer;

    public event Action<float> OnFillChange;

    public NeutralFillState(Action<IFillState> requestTransitionAction, float fillRate, float minHoldDuration, float maxHoldDuration)
    {
        m_requestTransitionAction = requestTransitionAction;
        m_fillRate = fillRate;
        m_minHoldDuration = minHoldDuration;
        m_maxHoldDuration = maxHoldDuration;
    }
    /// <inheritdoc/>
    public void OnEventStarted(float currentFill)
    {
        m_startingFill = currentFill;
        float driftDuration = Mathf.Abs(TargetFill - currentFill) / m_fillRate;
        if (driftDuration > 0f)
        {
            m_driftTimer = new(driftDuration);
            m_driftTimer.OnTimerTick += HandleDrift;
            m_driftTimer.OnTimerStop += OnDriftCompleted;
            m_driftTimer.Start();
        }
        else
        {
            OnDriftCompleted();
        }
    }
    /// <inheritdoc/>
    public void OnEventEnded()
    {
        m_driftTimer?.Stop();
        m_holdTimer?.Stop();
    }
    /// <inheritdoc/>
    public void HandleIncrease()
    {
        m_requestTransitionAction(OnIncrease);
    }
    /// <inheritdoc/>
    public void HandleDecrease()
    {
        m_requestTransitionAction(OnDecrease);
    }

    void HandleDrift() => OnFillChange?.Invoke(Mathf.Lerp(m_startingFill, TargetFill, 1 - m_driftTimer.Progress));

    void OnDriftCompleted()
    {
        float holdDuration = Random.Range(m_minHoldDuration, m_maxHoldDuration);
        m_holdTimer = new(holdDuration);
        m_holdTimer.OnTimerStop += OnHoldCompleted;
        m_holdTimer.Start();
    }

    void OnHoldCompleted()
    {
        bool increase = Random.value > 0.5f;
        m_requestTransitionAction(increase ? OnIncrease : OnDecrease);
    }
}