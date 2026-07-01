using System.Collections.Generic;
using UnityEngine;

public class CuttingBoardInteractable : MonoBehaviour, IInteractable, IPromptProvider
{
    InteractionSession m_currentInteractionSession;
    [SerializeField] private Transform _interactDisplayTransform;
    [SerializeField] private Transform storingLocation;

    private readonly string _widgetForPrompt = "interact";
    private IPlayerControllable _playerControllable;
    private IPlayerController _playerController;
    private PlayerInteractionState _playerInteractionState;
    private FoodClass _foodClassItem;
    private FoodClass currentFood;
    private Fish fish;
    private GameObject lastCookedObject;


    public void Update()
    {

    }
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        _playerControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();

        _playerController = _playerControllable.GetActivePlayerController();

        _playerInteractionState = _playerControllable.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();


        if (_playerInteractionState.CheckInteractionTag(InteractionTag.HoldingCookedFish))
        {
            _foodClassItem = _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectLocation>().GetComponentInChildren<FoodClass>();
            if (storingLocation.childCount == 0)
            {
                m_currentInteractionSession = new InteractionSession(interactor, this);
                m_currentInteractionSession.OnEnded += () => _playerController.ChangeControlledEntity(_playerControllable);
                MoveObjectToBoard();
                _playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingCookedFish);
                m_currentInteractionSession.End();
                return m_currentInteractionSession;
            }
        }
        else
        {
            if (storingLocation.childCount > 0)
            {
                MoveObjetToHand();
                m_currentInteractionSession = new InteractionSession(interactor, this);
                m_currentInteractionSession.OnEnded += () => _playerController.ChangeControlledEntity(_playerControllable);
                _playerInteractionState.AddInteractionTag(InteractionTag.HoldingCookedFish);
                m_currentInteractionSession.End();
                return m_currentInteractionSession;
            }
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
        return _interactDisplayTransform == null ? transform.position : _interactDisplayTransform.position;
    }

    private void MoveObjectToBoard()
    {
        _foodClassItem.transform.position = storingLocation.position;
        _foodClassItem.transform.SetParent(storingLocation);

    }


    private void MoveObjetToHand()
    {
        FoodClass cookingItem = storingLocation.GetComponentInChildren<FoodClass>();
        cookingItem.transform.position = _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectLocation>().transform.position;
        cookingItem.transform.SetParent(_playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectLocation>().transform);
        cookingItem.InitializeHungerAndThirst(_playerControllable.GetAssociatedGameObject().GetComponentInChildren<HungerAndThirst>());
    }
}
