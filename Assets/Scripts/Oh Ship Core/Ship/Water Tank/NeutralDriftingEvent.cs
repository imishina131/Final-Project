using System;
using MatrixUtils.Timers;

public class NeutralDriftingEvent : IDriftingEvent
{
    readonly Action m_onDriftStart;
    readonly float m_minHoldDuration;
    readonly float m_maxHoldDuration;
    CountdownTimer m_driftTimer;

    ~NeutralDriftingEvent()
    {
        m_driftTimer?.Dispose();
    }

    public NeutralDriftingEvent(Action onDriftStart, float minHoldDuration, float maxHoldDuration)
    {
        m_onDriftStart = onDriftStart;
        m_minHoldDuration = minHoldDuration;
        m_maxHoldDuration = maxHoldDuration;
    }

    public void OnEventStart(float currentFill)
    {
        float duration = UnityEngine.Random.Range(m_minHoldDuration, m_maxHoldDuration);
        m_driftTimer = new(duration);
        m_driftTimer.OnTimerStop += m_onDriftStart;
        m_driftTimer.Start();
    }

    public void OnEventUpdate(float currentFill) { }
    public void OnEventPaused(float currentFill) { }
    public void OnEventResumed(float currentFill) { }

    public void OnEventEnd(float currentFill)
    {
        m_driftTimer.OnTimerStop -= m_onDriftStart;
        m_driftTimer = null;
    }
}