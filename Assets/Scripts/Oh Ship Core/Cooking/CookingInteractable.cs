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
    private FoodClass currentFood;
    private Fish fish;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip dropCrabSound;
    [SerializeField] private AudioClip cookSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        Cook();
    }
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        _playerControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
       
        _playerController = _playerControllable.GetActivePlayerController();
        
        _playerInteractionState = _playerControllable.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();
         

        if (_playerInteractionState.CheckInteractionTag(InteractionTag.HoldingFish))
        {
            _foodClassItem = _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectLocation>().GetComponentInChildren<FoodClass>();
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
                _playerInteractionState.AddInteractionTag(InteractionTag.HoldingFish);
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
       return _interactDisplayTransform == null? transform.position : _interactDisplayTransform.position;
    }

    private void MoveObjectToStove()
    { 
        _foodClassItem.transform.position = cookingLocation.position;
        _foodClassItem.transform.SetParent(cookingLocation);
        if(_foodClassItem.CookingProcess == CookingProcess.InPot)
        {
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
        if(_foodClassItem == null || cookingLocation.childCount <= 0        )
        {
            return;
        }

        if(cookingLocation.childCount > 0)
        {
            cookedAmount += _foodClassItem.GetCookingSpeed() * Time.deltaTime;
            _foodClassItem.UpdateCookedAmount(cookedAmount);
        }
    }

    private void MoveObjetToHand()
    {
        audioSource.Stop();
        FoodClass cookingItem = cookingLocation.GetComponentInChildren<FoodClass>();
        cookingItem.transform.position = _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectLocation>().transform.position;
        cookingItem.transform.SetParent(_playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectLocation>().transform);
        cookingItem.InitializeHungerAndThirst(_playerControllable.GetAssociatedGameObject().GetComponentInChildren<HungerAndThirst>());
    }

    private void PlaySound()
    {
        audioSource.clip = cookSound;
        audioSource.Play();
    }
    
}
