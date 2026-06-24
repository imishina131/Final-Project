using System;
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

    private bool drinking = false;
    [SerializeField] private float drinkingRate = 0.4f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(drinking)
        {
            DrinkWater();
        }
    }

    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        _playerControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        thirstManager = _playerControllable.GetAssociatedGameObject().GetComponent<HungerAndThirst>();

        _playerController = _playerControllable.GetActivePlayerController();

        _playerInteractionState = _playerControllable.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();

        if (_playerInteractionState.CheckInteractionTag(InteractionTag.Holding))
        {
            return null;
        }
        else
        {
            if(animSink.GetBool("waterRunning"))
            {
                drinking = false;
                animSink.SetBool("waterRunning", false);
            }
            else
            {
                drinking = true;
                animSink.SetBool("waterRunning", true);
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

    void DrinkWater()
    {
        thirstManager.Thirst.Value += drinkingRate * Time.deltaTime;
    }
}
