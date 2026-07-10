using System;
using System.Collections;
using MatrixUtils.DependencyInjection;
using UnityEngine;

public class CuttingBoardInteractable : MonoBehaviour, IInteractable, IPromptProvider
{
    InteractionSession m_currentInteractionSession;
    [SerializeField] private Transform _interactDisplayTransform;
    [SerializeField] private Transform storingLocation;

    private readonly string _widgetForPrompt = "interact";
   // private IPlayerControllable _playerControllable;
  //  private IPlayerController _playerController;
   // private PlayerInteractionState _playerInteractionState;
    private FoodClass _foodClassItem;
    private FoodClass currentFood;
    private Fish fish;
    private GameObject lastCookedObject;

    [Header ("Audio")]
    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip pickUp;
    [SerializeField] AudioClip putDown;

    [Inject] INotificationMessenger m_notificationMessenger;

    private void Awake()
    {
        FindAnyObjectByType<Injector>().Inject(this);
        audio = GetComponent<AudioSource>();
    }

    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        IPlayerControllable  _playerControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();

        IPlayerController  _playerController = _playerControllable.GetActivePlayerController();

         PlayerInteractionState _playerInteractionState = _playerControllable.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();


        if (_playerInteractionState.CheckInteractionTag(InteractionTag.HoldingCookedFish))
        {
            _foodClassItem = _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectHandler>().GetComponentInChildren<FoodClass>();
            if (storingLocation.childCount == 0)
            {
                m_currentInteractionSession = new InteractionSession(interactor, this);
                m_currentInteractionSession.OnEnded += () => _playerController.ChangeControlledEntity(_playerControllable);
                MoveObjectToBoard();
                _playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingCookedFish);
                _playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingFish);
                m_currentInteractionSession.End();
                return m_currentInteractionSession;
            }
        }

        if (_playerInteractionState.CheckInteractionTag(InteractionTag.HoldingFish) ||
            _playerInteractionState.CheckInteractionTag(InteractionTag.HoldingCookedFish) ||
            _playerInteractionState.CheckInteractionTag(InteractionTag.HoldingBottle) ||
            _playerInteractionState.CheckInteractionTag(InteractionTag.HoldingBottleWithWater))
        {
            m_currentInteractionSession = new InteractionSession(interactor, this);
            m_currentInteractionSession.End();
            return m_currentInteractionSession;
        }
        
        if (storingLocation.childCount > 0)
        {
            MoveObjetToHand(_playerControllable);
            m_currentInteractionSession = new InteractionSession(interactor, this);
            m_currentInteractionSession.OnEnded += () => _playerController.ChangeControlledEntity(_playerControllable);
            _playerInteractionState.AddInteractionTag(InteractionTag.HoldingCookedFish);
            m_currentInteractionSession.End();
            return m_currentInteractionSession;
        }
        

        m_currentInteractionSession = new InteractionSession(interactor, this);
        m_currentInteractionSession.End();

        StartCoroutine(DisplayWarning(_playerInteractionState.PlayerIndex));
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
        audio.PlayOneShot(putDown);
        _foodClassItem.transform.position = storingLocation.position;
        _foodClassItem.transform.SetParent(storingLocation);
        if (_foodClassItem.GetComponentInChildren<Fish>())
        {
            _foodClassItem.transform.localRotation = Quaternion.Euler(1.2f, 88.7f, 91.4f);
        }
        else if (_foodClassItem.GetComponentInChildren<Crab>())
        {
            _foodClassItem.transform.localRotation = Quaternion.Euler(186.8f, 181.7f, 3.2f);
        }
        Debug.Log("foodclass:" + _foodClassItem.transform.position);
    }


    private void MoveObjetToHand(IPlayerControllable playerControllable)
    {
        Debug.Log($"MoveObjetToHand called for: {playerControllable.GetAssociatedGameObject().name}");
        audio.PlayOneShot(pickUp);
        FoodClass cookingItem = storingLocation.GetComponentInChildren<FoodClass>();
        cookingItem.transform.position = playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectHandler>().transform.position;
        cookingItem.transform.SetParent(playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectHandler>().transform);
        if (_foodClassItem.GetComponentInChildren<Fish>())
        {
            cookingItem.transform.localRotation = Quaternion.Euler(44.1f, 142f, 77.5f);
        }
        else
        {
            cookingItem.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        HungerAndThirst hunger = playerControllable.GetAssociatedGameObject().GetComponentInChildren<HungerAndThirst>();
        Debug.Log($"HungerAndThirst found on: {hunger?.gameObject.name}");
        cookingItem.InitializeHungerAndThirst(hunger);
        //cookingItem.InitializeHungerAndThirst(playerControllable.GetAssociatedGameObject().GetComponentInChildren<HungerAndThirst>());
    }
    
    IEnumerator DisplayWarning(int playerIndex)
    {
        Debug.Log("Warning Label");
       // int playerIndex = _playerInteractionState.PlayerIndex;
        Debug.Log($"Firing: 'enable cooked player{playerIndex}'");
        Debug.Log(m_notificationMessenger);
        m_notificationMessenger.TryNotify($"enable cooked player{playerIndex}");
        yield return new WaitForSeconds(3f);
        m_notificationMessenger.TryNotify($"disable cooked player{playerIndex}");;
    }
}
