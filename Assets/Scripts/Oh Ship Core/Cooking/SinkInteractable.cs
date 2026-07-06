using System;
using System.Collections;
using System.Threading;
using MatrixUtils.DependencyInjection;
using UnityEngine;

public class SinkInteractable : MonoBehaviour, IInteractable, IPromptProvider
{
    InteractionSession m_currentInteractionSession;
    [SerializeField] private Transform _interactDisplayTransform;

    private readonly string _widgetForPrompt = "interact";
    private IPlayerControllable _playerControllable;
    private IPlayerController _playerController;
    private PlayerInteractionState _playerInteractionState;
    public Animator animSink;

    private HungerAndThirst _thirstManager;

    [Inject] INotificationMessenger m_notificationMessenger;
    private bool fillingUp = false;
    private bool drinking = false;
    [SerializeField] private float drinkingRate = 0.4f;
    [SerializeField] private float cooldown = 3.0f;
    private float timer = 0f;
    private float amountOfWater = 0f;
    private bool canInteract = true;
    private bool filledUp = false;

    private IPlayerControllable _playerControllableForHoldingObject;
    private Transform _holdingObjectTransform;
    private GameObject heldObject;
    [SerializeField] private GameObject bottleToSpawn;
    [SerializeField] private GameObject bottleInSink;
    [SerializeField] private GameObject bottleBack;

    private AudioSource _audioSource;
   
    [Header("Sound")] 
    [SerializeField] private AudioClip drink;
    [SerializeField] private AudioClip waterSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        FindAnyObjectByType<Injector>().Inject(this);
        //_audioSource.clip = drink;
    }

    // Update is called once per frame
    void Update()
    {
        timer = timer += 1 * Time.deltaTime;
        //Debug.Log("water" + amountOfWater);

        if(timer >= cooldown)
        {
            canInteract = true;
        }
        if(fillingUp)
        {
            if(!animSink.GetBool("waterRunning"))
            {
                animSink.SetBool("waterRunning", true);
                _audioSource.Play();
            }
            amountOfWater = amountOfWater += 0.1f * Time.deltaTime;
            if (amountOfWater >= 1f)
            {

                timer = 0f;
                canInteract = false;
                fillingUp = false;
                animSink.SetBool("waterRunning", false);
                _audioSource.Stop();
                filledUp = true;
            }
        }
    }

    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        IPlayerControllable oldControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();

        _thirstManager = oldControllable.GetAssociatedGameObject().GetComponent<HungerAndThirst>();

     
        _playerInteractionState = oldControllable.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();

       
        if(_playerInteractionState.CheckInteractionTag(InteractionTag.HoldingBottle))
        {
            _playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingBottle);
            bottleInSink.SetActive(true);
            _holdingObjectTransform = oldControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectHandler>().transform;
            if (_holdingObjectTransform.childCount > 0)
            {
                Destroy(_holdingObjectTransform.GetChild(0).gameObject);
            }
            timer = 0f;
            fillingUp = true;
        }
       
        else if(filledUp || fillingUp) 
        {
            //Debug.Log("Doesn't contain bottle in hand?");
            _playerInteractionState.AddInteractionTag(InteractionTag.HoldingBottleWithWater);
            _audioSource.clip = drink;
            _audioSource.PlayOneShot(drink);
            DrinkWater(amountOfWater);
            bottleInSink.SetActive(false);
            bottleBack.SetActive(true);
            filledUp = false;
            fillingUp = false;
            animSink.SetBool("waterRunning", false);
            _audioSource.Stop();
            timer = 0f;
        }
        else
        {
            StartCoroutine(PlayerNotificationBlock());
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


    void DrinkWater(float toDrink)
    {
        _audioSource.clip = waterSound;
        _thirstManager.Thirst.Value += toDrink;
        amountOfWater = 0f;
        _playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingBottleWithWater);
        Debug.Log("Removed HoldingBottleWithWater");
    }

    IEnumerator PlayerNotificationBlock()
    {
        int playerIndex = _playerInteractionState.PlayerIndex;
        m_notificationMessenger.TryNotify($"enable bottle player{playerIndex}");
        yield return new WaitForSeconds(1.2f);
        m_notificationMessenger.TryNotify($"disable bottle player{playerIndex}");
    }
}
