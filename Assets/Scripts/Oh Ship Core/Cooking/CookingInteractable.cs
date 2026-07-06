using System.Collections;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.InputSystem;

public class CookingInteractable : MonoBehaviour, IInteractable, IPromptProvider
{
    InteractionSession m_currentInteractionSession;
    [SerializeField] private Transform _interactDisplayTransform;
    [SerializeField] private Transform cookingLocation;
    [SerializeField] private CookingProcess howIsCooked;
    
    private readonly string _widgetForPrompt = "interact";
    private IPlayerControllable _playerControllable;
    private IPlayerController _playerController;
    private PlayerInteractionState _playerInteractionState;
    private FoodClass _foodClassItem;
    private float cookedAmount;
    private bool hasDropped = false;
    private GameObject lastCookedObject;
    private Transform holdingRotation;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip dropCrabSound;
    [SerializeField] private AudioClip cookSound;

    [Inject] INotificationMessenger m_notificationMessenger;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        Cook();

        if(cookedAmount >= 1)
        {
            audioSource.Stop();
        }
    }
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        _playerControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
       
        _playerController = _playerControllable.GetActivePlayerController();
        
        _playerInteractionState = _playerControllable.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();
         

        if (_playerInteractionState.CheckInteractionTag(InteractionTag.HoldingFish))
        {
            _foodClassItem = _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectHandler>().GetComponentInChildren<FoodClass>();
            if (_foodClassItem.CookingProcess == howIsCooked && cookingLocation.childCount == 0)
            {
                m_currentInteractionSession = new InteractionSession(interactor, this);
                m_currentInteractionSession.OnEnded += () => _playerController.ChangeControlledEntity(_playerControllable);
                MoveObjectToStove();
                _playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingFish);
                m_currentInteractionSession.End();
                return m_currentInteractionSession;
            }
        }
        else
        {
            if (cookingLocation.childCount > 0)
            {
                MoveObjetToHand();
                m_currentInteractionSession = new InteractionSession(interactor, this);
                m_currentInteractionSession.OnEnded += () => _playerController.ChangeControlledEntity(_playerControllable);

                switch (_foodClassItem.CookStateRef)
                {
                    case CookState.Cooked:
                        _playerInteractionState.AddInteractionTag(InteractionTag.HoldingCookedFish);
                        break;
                    case CookState.Raw:
                        _playerInteractionState.AddInteractionTag(InteractionTag.HoldingFish);
                        break;
                    case CookState.Burnt:
                        _playerInteractionState.AddInteractionTag(InteractionTag.HoldingCookedFish);
                        break;
                }
                _playerInteractionState.AddInteractionTag(InteractionTag.HoldingFish);
                m_currentInteractionSession.End();
                return m_currentInteractionSession;
            }
        }
        
        m_currentInteractionSession = new InteractionSession(interactor, this);
        m_currentInteractionSession.End();
        StartCoroutine(DisplayWarning());
       
        return m_currentInteractionSession;
       
    }

    public PromptData GetPromptData()
    {
        return new PromptData { AssociatedWidget = _widgetForPrompt };
    }

    public Vector3 GetWidgetWorldPosition()
    {
       return _interactDisplayTransform == null? transform.position : _interactDisplayTransform.position;
    }

    private void MoveObjectToStove()
    { 
        _foodClassItem.transform.position = cookingLocation.position;
        _foodClassItem.transform.SetParent(cookingLocation);
        holdingRotation = _foodClassItem.transform;
        if (_foodClassItem.GetComponentInChildren<Fish>())
        {
            _foodClassItem.transform.localRotation = Quaternion.Euler(1.4f, -0.6f, 88.8f);
        }
        //_foodClassItem.transform.localRotation = Quaternion.Euler(1.2f, 88.7f, 91.4f);

        if (_foodClassItem.CookingProcess == CookingProcess.InPot && !hasDropped)
        {
            hasDropped = true;
            audioSource.PlayOneShot(dropCrabSound);
            Invoke("PlaySound", 1.5f);
        }
        else if (_foodClassItem.CookingProcess == CookingProcess.OnGrill)
        {
            PlaySound();
        }
    }

    private void Cook()
    {
        if(_foodClassItem == null)
        {
            return;
        }

        if(cookingLocation.childCount > 0 && _foodClassItem.transform.position == cookingLocation.position)
        {
            if(lastCookedObject != _foodClassItem.gameObject)
            {
                cookedAmount = 0f;
                lastCookedObject = _foodClassItem.gameObject;
            }
            cookedAmount += _foodClassItem.GetCookingSpeed() * Time.deltaTime;
            _foodClassItem.UpdateCookedAmount(cookedAmount);
        }
    }

    private void MoveObjetToHand()
    {
        audioSource.Stop();
        hasDropped = false;
        FoodClass cookingItem = cookingLocation.GetComponentInChildren<FoodClass>();
        cookingItem.transform.position = _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectHandler>().transform.position;
        cookingItem.transform.SetParent(_playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectHandler>().transform);
        if (_foodClassItem.GetComponentInChildren<Fish>())
        {
            cookingItem.transform.localRotation = Quaternion.Euler(44.1f, 142f, 77.5f);
        }
        else
        {
            cookingItem.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        cookingItem.InitializeHungerAndThirst(_playerControllable.GetAssociatedGameObject().GetComponentInChildren<HungerAndThirst>());
        if(cookedAmount > 0.3f)
        {
            _playerInteractionState.AddInteractionTag(InteractionTag.HoldingCookedFish);
        }
    }

    private void PlaySound()
    {
        audioSource.PlayOneShot(cookSound);

    }

    IEnumerator DisplayWarning()
    {
        Debug.Log("Warning Label");
        int playerIndex = _playerInteractionState.PlayerIndex;
        Debug.Log($"Firing: 'enable raw player{playerIndex}'");
        m_notificationMessenger.TryNotify($"enable raw player{playerIndex}");
        yield return new WaitForSeconds(3f);
        m_notificationMessenger.TryNotify($"disable raw player{playerIndex}");;
    }
    
}
