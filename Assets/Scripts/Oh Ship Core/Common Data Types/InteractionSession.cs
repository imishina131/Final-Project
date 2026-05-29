using System;
using UnityEngine.Events;

/// <summary>
/// Represents an ongoing interaction between an <see cref="IInteractor"/> and an <see cref="IInteractable"/>
/// </summary>
public class InteractionSession
{
    /// <summary>
    /// The <see cref="IInteractor"/> that is currently interacting with the <see cref="Target"/>
    /// </summary>
    public IInteractor CurrentInteractor { get; private set; }
    /// <summary>
    /// The <see cref="IInteractable"/> that is being interacted with.
    /// </summary>
    public IInteractable Target { get; private set; }
    /// <summary>
    /// Whether the interaction session is still active.
    /// </summary>
    public bool IsActive { get; private set; }
    /// <summary>
    /// Invoked when the <see cref="InteractionSession"/> ends.
    /// </summary>
    public event Action OnEnded;
    /// <summary>
    /// Invoked when the <see cref="InteractionSession"/> is transferred to a new <see cref="IInteractor"/>. First parameter is the old <see cref="IInteractor"/>, second parameter is the new <see cref="IInteractor"/>
    /// </summary>
    public event Action<IInteractor, IInteractor> OnTransferred;
    /// <summary>
    /// Creates a new <see cref="InteractionSession"/>
    /// </summary>
    /// <param name="interactor">The <see cref="IInteractor"/> doing the interaction</param>
    /// <param name="target">The target <see cref="IInteractable"/> for the session</param>
    public InteractionSession(IInteractor interactor, IInteractable target)
    {
        CurrentInteractor =interactor;
        Target = target;
        IsActive = true;
    }
    /// <summary>
    /// Transfers the interaction session to a new <see cref="IInteractor"/>
    /// </summary>
    /// <param name="newInteractor">The <see cref="IInteractor"/> to transfer ownership to</param>
    public bool TransferTo(IInteractor newInteractor)
    {
        if (!IsActive || newInteractor is null) return false;
        IInteractor old = CurrentInteractor;
        CurrentInteractor = newInteractor;
        OnTransferred?.Invoke(old, newInteractor);
        return true;
    }
    /// <summary>
    /// Ends the <see cref="InteractionSession"/>
    /// </summary>
    public void End()
    {
        if (!IsActive) return;
        IsActive = false;
        OnEnded?.Invoke();
        OnEnded = null;
        OnTransferred = null;
    }
}