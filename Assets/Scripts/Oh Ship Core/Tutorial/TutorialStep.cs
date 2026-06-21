using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents a step in the tutorial sequence. These are used to represent the different stages of the tutorial. Usually handled by the <see cref="TutorialOrchestrator"/>
/// </summary>
[Serializable]
public abstract class TutorialStep
{
    /// <summary>
    /// Starts the tutorial step.
    /// </summary>
    public void StartStep()
    {
        StartStepInternal();
        m_onStepStart.Invoke();
    }
    /// <summary>
    /// Ends the tutorial step.
    /// </summary>
    public void EndStep()
    {
        EndStepInternal();
        m_onStepComplete.Invoke();
    }
    /// <summary>
    /// Override this to implement the step's starting logic. Will be called by <see cref="StartStep"/>
    /// </summary>
    protected abstract void StartStepInternal();
    /// <summary>
    /// Override this to implement the step's ending logic. Will be called by <see cref="EndStep"/>
    /// </summary>
    protected abstract void EndStepInternal();
    /// <summary>
    /// Invoked when the step starts.
    /// </summary>
    [SerializeField] UnityEvent m_onStepStart = new();
    /// <summary>
    /// Try to add a listener to the start event.
    /// </summary>
    /// <param name="event">The event to try and add</param>
    /// <returns>Whether the event was successfully added</returns>
    public bool TryAddStartEvent(UnityAction @event)
    {
        if (@event == null) return false;
        m_onStepComplete.AddListener(@event);
        return true;
    }
    /// <summary>
    /// Try to remove a listener from the start event.
    /// </summary>
    /// <param name="event">The event to try and add</param>
    /// <returns>Whether the event was successfully removed</returns>
    public bool TryRemoveStartEvent(UnityAction @event)
    {
        if (@event == null) return false;
        m_onStepComplete.RemoveListener(@event);
        return true;
    }
    
    [SerializeField] UnityEvent m_onStepComplete = new();
    /// <summary>
    /// Try to add a listener to the complete event.
    /// </summary>
    /// <param name="event">The event to try and add</param>
    /// <returns>Whether the event was successfully added</returns>
    public bool TryAddCompleteEvent(UnityAction @event)
    {
        if (@event == null) return false;
        m_onStepComplete.AddListener(@event);
        return true;
    }
    /// <summary>
    /// Try to remove a listener from the complete event.
    /// </summary>
    /// <param name="event">The event to try and add</param>
    /// <returns>Whether the event was successfully removed</returns>
    public bool TryRemoveCompleteEvent(UnityAction @event)
    {
        if (@event == null) return false;
        m_onStepComplete.RemoveListener(@event);
        return true;
    }
}