using System;
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
    [SerializeField] LayerMask m_interactionLayer;
    InteractionSession m_session;
    HeldObjectLocation m_heldObjectLocation;
    
    public bool WasInteracting { get; private set; }
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
    /// <inheritdoc/>
    public GameObject GetAssociatedGameObject() => gameObject;

    /// <summary>
    /// Attempts to begin an interaction with the nearest <see cref="IInteractable"/> in the <see cref="m_interactionRange"/>
    /// </summary>
    public void OnInteractionButtonPressed()
    {
        if (IsInteracting())
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit checkHit, m_interactionRange, m_interactionLayer) 
                && checkHit.collider.TryGetComponent(out IInteractable checkInteractable))
            {
                WasInteracting = IsInteracting();
                EndActiveInteraction();
                m_session = checkInteractable.BeginInteraction(this);
                if (m_session is not { IsActive: true }) return;
                SubscribeToSession();
                return;
            }
            EndActiveInteraction();
            return;
        }
        WasInteracting = false;
        if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, m_interactionRange, m_interactionLayer)) return;
        if (!hit.collider.TryGetComponent(out IInteractable interactable)) return;
        m_session = interactable.BeginInteraction(this);
        
        if (m_session is not { IsActive: true }) return;
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
        session.End();
    }
    void OnDisable() => EndActiveInteraction();
    
    private void Start()
    {
        m_heldObjectLocation = GetComponentInChildren<HeldObjectLocation>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}