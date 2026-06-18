using System;
using MatrixUtils.Timers;
using UnityEngine;
/// <summary>
/// A fill state where the tank is filling or draining and is moving towards the target fill position
/// </summary>
public class ChangeFillState : IFillState
{
    readonly Action<IFillState> m_requestTransitionAction;
    readonly float m_targetFill;
    readonly float m_fillRate;
    public IFillState OnIncrease;
    public IFillState OnDecrease;
    CountdownTimer m_timer;
    float m_startingFill;

    public event Action<float> OnFillChange;

    public ChangeFillState(Action<IFillState> requestTransitionAction, float targetFill, float fillRate)
    {
        m_requestTransitionAction = requestTransitionAction;
        m_targetFill = targetFill;
        m_fillRate = fillRate;
    }
    /// <inheritdoc/>
    public void OnEventStarted(float startingFill)
    {
        m_startingFill = startingFill;
        float duration = Mathf.Abs(startingFill - m_targetFill) / m_fillRate;
        if (duration <= 0f)
        {
            m_requestTransitionAction(OnIncrease);
            return;
        }
        m_timer = new(duration);
        m_timer.OnTimerTick += HandleFillChange;
       // m_timer.OnTimerStop += () => OnFillChange?.Invoke(m_targetFill);
        m_timer.Start();
    }
    /// <inheritdoc/>
    public void OnEventEnded() => m_timer?.Stop();
    /// <inheritdoc/>
    public void HandleIncrease() => m_requestTransitionAction(OnIncrease);
    /// <inheritdoc/>
    public void HandleDecrease() => m_requestTransitionAction(OnDecrease);
    
    void HandleFillChange() => OnFillChange?.Invoke(Mathf.Lerp(m_startingFill, m_targetFill, 1 - m_timer.Progress));
}