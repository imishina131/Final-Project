using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles interactions with <see cref="IInteractable"/> by the player.
/// </summary>
public class PlayerInteractor : MonoBehaviour, IInteractor
{
    /// <summary>
    /// The range in which the player can interact with an <see cref="IInteractable"/>
    /// </summary>
    [SerializeField] float m_interactionRange = 2;
    InteractionSession m_session;
    
    /// <inheritdoc/>
    public bool IsInteracting() => m_session?.IsActive is true;
    /// <inheritdoc/>
    public InteractionSession GetSession() => m_session;
    /// <inheritdoc/>
    public bool RequestSessionTransfer(InteractionSession session)
    {
        if (IsInteracting() || !session.TransferTo(this)) return false;
        m_session = session;
        SubscribeToSession();
        return true;
    }
    /// <summary>
    /// Attempts to begin an interaction with the nearest <see cref="IInteractable"/> in the <see cref="m_interactionRange"/>
    /// </summary>
    public void OnInteractionButtonPressed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (IsInteracting())
        {
            EndActiveInteraction();
            return;
        }
        if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, m_interactionRange)) return;
        if (!hit.collider.TryGetComponent(out IInteractable interactable)) return;
        m_session = interactable.BeginInteraction(this);
        SubscribeToSession();
    }
    void SubscribeToSession()
    {
        m_session.OnEnded +=  () => m_session = null;
        m_session.OnTransferred += (old, _) =>
        {
            if (!ReferenceEquals(old, this)) return;
            m_session = null;
        };
    }
    /// <summary>
    /// Ends the current <see cref="InteractionSession"/> if one exists.
    /// </summary>
    public void EndActiveInteraction()
    {
        if (!IsInteracting()) return;
        InteractionSession session = m_session;
        m_session = null;
        session.Target.EndInteraction(session);
        session.End();
    }
    void OnDisable() => EndActiveInteraction();
}