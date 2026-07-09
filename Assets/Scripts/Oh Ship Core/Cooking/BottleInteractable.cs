using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BottleInteractable : MonoBehaviour, IInteractable, IPromptProvider
{
    InteractionSession m_currentInteractionSession;
    [SerializeField] private Transform _interactDisplayTransform;

    private readonly string _widgetForPrompt = "interact";
    private IPlayerControllable _playerControllable;
    private IPlayerController _playerController;
    private PlayerInteractionState _playerInteractionState;
    [SerializeField] GameObject _bottleProp;

    private IPlayerControllable _playerControllableForHoldingObject;
    private Transform _holdingObjectTransform;
    [SerializeField] private GameObject bottleToSpawn;
    [SerializeField] private GameObject bottleTaken;

    [Header("Audio")]
    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip pickUp;
    
 
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        IPlayerControllable playerControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        PlayerInteractionState playerInteractionState = playerControllable.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();

        if (playerInteractionState.CheckInteractionTag(InteractionTag.HoldingFish) || 
            playerInteractionState.CheckInteractionTag(InteractionTag.HoldingBottle) || 
            playerInteractionState.CheckInteractionTag(InteractionTag.HoldingCookedFish))
            return null;

        playerInteractionState.AddInteractionTag(InteractionTag.HoldingBottle);
        audio.PlayOneShot(pickUp);
        Transform holdingTransform = playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectHandler>().transform;
        GameObject bottle = Instantiate(_bottleProp, holdingTransform.position, holdingTransform.rotation);
        bottle.transform.SetParent(holdingTransform);
        gameObject.SetActive(false);

        m_currentInteractionSession = new InteractionSession(interactor, this);
        m_currentInteractionSession.End();
        return m_currentInteractionSession;
    }

    public PromptData GetPromptData()
    {
        if (m_currentInteractionSession is { IsActive: true })
        {
            return new PromptData { AssociatedWidget = "" };
        }
        return new PromptData { AssociatedWidget = _widgetForPrompt };
    }

    public Vector3 GetWidgetWorldPosition()
    {
        return _interactDisplayTransform == null ? transform.position : _interactDisplayTransform.position;
    }
}
