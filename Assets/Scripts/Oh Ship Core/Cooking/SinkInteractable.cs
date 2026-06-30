using System;
using System.Collections;
using System.Threading;
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

    [SerializeField] HungerAndThirst thirstManager;
    //[SerializeField] StatusBar m_thirstBar;

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

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        timer = timer += 1 * Time.deltaTime;
        Debug.Log("water" + amountOfWater);

        if(timer >= cooldown)
        {
            canInteract = true;
        }
        if(fillingUp)
        {
            if(!animSink.GetBool("waterRunning"))
            {
                animSink.SetBool("waterRunning", true);
                audioSource.Play();
            }
            amountOfWater = amountOfWater += 0.1f * Time.deltaTime;
            if (amountOfWater >= 1f)
            {

                timer = 0f;
                canInteract = false;
                fillingUp = false;
                animSink.SetBool("waterRunning", false);
                audioSource.Stop();
                filledUp = true;
            }
        }
    }

    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        _playerControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        thirstManager = _playerControllable.GetAssociatedGameObject().GetComponent<HungerAndThirst>();

        IPlayerControllable oldControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        IPlayerController controller = oldControllable.GetActivePlayerController();
        _playerControllableForHoldingObject = oldControllable;

        _playerController = _playerControllable.GetActivePlayerController();

        _playerInteractionState = _playerControllable.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();

        if (_playerInteractionState.CheckInteractionTag(InteractionTag.HoldingFish))
        {
            return null;
        }
        else if(_playerInteractionState.CheckInteractionTag(InteractionTag.HoldingBottle))
        {
            _playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingBottle);
            bottleInSink.SetActive(true);
            _holdingObjectTransform = _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectLocation>().transform;
            if (_holdingObjectTransform.childCount > 0)
            {
                Destroy(_holdingObjectTransform.GetChild(0).gameObject);
            }
            timer = 0f;
            fillingUp = true;
        }
        else if (!_playerInteractionState.CheckInteractionTag(InteractionTag.HoldingBottle))
        {
            DrinkWater(amountOfWater);
            bottleInSink.SetActive(false);
            bottleBack.SetActive(true);
            filledUp = false;
            fillingUp = false;
            animSink.SetBool("waterRunning", false);
            audioSource.Stop();
            timer = 0f;
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
        thirstManager.Thirst.Value += toDrink;
        amountOfWater = 0f;
    }
}
