using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PassOutInteractable : MonoBehaviour, IInteractable, IPromptProvider
{
    InteractionSession m_currentInteractionSession;
    [SerializeField] private Transform _interactDisplayTransform;
    private readonly string _widgetForPrompt = "interact";
    private IPlayerControllable _playerControllable;
    private IPlayerController _playerController;
    private PlayerInteractionState _playerInteractionState;
    private PlayerInteractor playerInteractor;

    private IPlayerControllable _playerControllable02;
    private IPlayerController _playerController02;
    private PlayerInteractionState _playerInteractionState02;

    private HungerAndThirst _hungerNThirst;

    HeldObjectLocation m_heldObjectLocation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _hungerNThirst = GetComponent<HungerAndThirst>();
        Debug.Log(_hungerNThirst);
    }

    void Update()
    {

    }
    
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        _playerControllable = GetComponent<PlayerControlRouter>();
        _playerController = _playerControllable.GetActivePlayerController();
        _playerInteractionState = _playerControllable.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();

        _playerControllable02 = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        _playerController02 = _playerControllable02.GetActivePlayerController();
        _playerInteractionState02 = _playerControllable02.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();

        m_heldObjectLocation = interactor.GetAssociatedGameObject().transform.parent.GetComponentInChildren<HeldObjectLocation>();

        if (_hungerNThirst.IsPassedOut)
        {   
            Debug.Log("went through pass");
            m_currentInteractionSession = new InteractionSession(interactor, this);
            m_currentInteractionSession.OnEnded += () => _playerController.ChangeControlledEntity(_playerControllable);
            IUsableItem usableItem = m_heldObjectLocation.GetComponentInChildren<IUsableItem>();

            if (_playerInteractionState02.CheckInteractionTag(InteractionTag.HoldingCookedFish))
            {
                _hungerNThirst.WakeUp(1f); 
                _playerInteractionState02.RemoveInteractionTag(InteractionTag.HoldingCookedFish);
            }
            else if (_playerInteractionState02.CheckInteractionTag(InteractionTag.HoldingFish))
            {
                _hungerNThirst.WakeUp(0.5f);
                _playerInteractionState02.RemoveInteractionTag(InteractionTag.HoldingFish);
            }
            else
            {
                Debug.Log("should wake up");
                _hungerNThirst.WakeUp(0.2f);
            }

            if (usableItem != null)
            {
                usableItem.Use();
            }
            return m_currentInteractionSession;
        }

        m_currentInteractionSession = new InteractionSession(interactor, this);
        m_currentInteractionSession.End();

        return m_currentInteractionSession;

    }
    
    public PromptData GetPromptData()
    {
        return new PromptData { AssociatedWidget = _widgetForPrompt };
    }

    public Vector3 GetWidgetWorldPosition()
    {
        return _interactDisplayTransform.position;
    }


}
