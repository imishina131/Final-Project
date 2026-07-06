using System.Collections.Generic;
using UnityEngine;

public class StorageInteractable : MonoBehaviour, IInteractable, IPromptProvider
{
    private IPlayerControllable _playerControllable;
    private IPlayerController _playerController;
    private Transform _holdingObjectTransform;
    InteractionSession m_currentInteractionSession;
    [SerializeField] private List<SO_CookableFoodData> _storedWaterLife =  new List<SO_CookableFoodData>();
    [SerializeField] private string _widgetForPrompt = "interact";
    [SerializeField] private int maxStoredFish = 6;
    private PlayerInteractionState _playerInteractionState;

    [SerializeField] private GameObject[] fishes = new GameObject[3];
    [SerializeField] private GameObject[] crabs = new GameObject[3];

    private int fishDisplayed = 0;
    private int crabsDisplayed = 0;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSFX;
    [SerializeField] private AudioClip drop;
    [SerializeField] private AudioClip pickup;

    private void Awake()
    {
        audioSFX = GetComponent<AudioSource>();
    }
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        _playerControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
       
        _playerController = _playerControllable.GetActivePlayerController();
        
        _playerInteractionState = _playerControllable.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();
        
        if (_storedWaterLife.Count < maxStoredFish && _playerInteractionState.CheckInteractionTag(InteractionTag.HoldingFish))
        {
            if (_playerControllable.GetAssociatedGameObject().GetComponentInChildren<FoodClass>().CookStateRef != CookState.Raw)
            {
                return null;
            }
            _holdingObjectTransform =  _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectHandler>().transform;
            AddFishToStorage(_playerControllable.GetAssociatedGameObject().GetComponentInChildren<FoodClass>().FoodData);
            Destroy(_holdingObjectTransform.GetChild(0).gameObject);
            m_currentInteractionSession = new InteractionSession(interactor,this);
            _playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingFish);
            m_currentInteractionSession.End();
            return m_currentInteractionSession;
        }

        if (_storedWaterLife.Count == 0)
        {
            m_currentInteractionSession = new InteractionSession(interactor, this);
            m_currentInteractionSession.End();
            return m_currentInteractionSession;
        }
        m_currentInteractionSession = new InteractionSession(interactor, this);
        
        RemoveFishFromStorage(_storedWaterLife[0]);
        
        return m_currentInteractionSession;
    }
    
    public void AddFishToStorage(SO_CookableFoodData foodData)
    {
        _storedWaterLife.Add(foodData);
        audioSFX.PlayOneShot(drop);
        if(_playerControllable.GetAssociatedGameObject().GetComponentInChildren<Fish>() != null)
        {
            fishDisplayed += 1;
        }
        else if (_playerControllable.GetAssociatedGameObject().GetComponentInChildren<Crab>() != null)
        {
            crabsDisplayed += 1;
        }
    }

    public void RemoveFishFromStorage(SO_CookableFoodData foodData)
    {
        audioSFX.PlayOneShot(pickup);
        HungerAndThirst hungerRef = _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HungerAndThirst>();
        _holdingObjectTransform =  _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectHandler>().transform;
        GameObject fish = Instantiate(foodData.Model, _holdingObjectTransform.position,_holdingObjectTransform.rotation);
        fish.transform.SetParent(_holdingObjectTransform);
        fish.GetComponent<FoodClass>().InitializeHungerAndThirst(hungerRef);
        _playerInteractionState.AddInteractionTag(InteractionTag.HoldingFish);
        _storedWaterLife.RemoveAt(0);
    }


    public PromptData GetPromptData()
    {
        return new PromptData() { AssociatedWidget = _widgetForPrompt, };
    }

    public Vector3 GetWidgetWorldPosition()
    {
        return transform.position;
    }

    private void Update()
    {
        UpdateCrabCount();
        UpdateFishCount();
    }

    private void UpdateFishCount()
    {
        
        switch (fishDisplayed)
        {
            case (1):
                fishes[0].SetActive(true);
                break;
            case (2):
                fishes[0].SetActive(true);
                fishes[1].SetActive(true);
                break;
            case (3):
                fishes[0].SetActive(true);
                fishes[1].SetActive(true);
                fishes[2].SetActive(true);
                break;
        }
        
    }

    private void UpdateCrabCount()
    {
        switch (crabsDisplayed)
        {
            case (1):
                crabs[0].SetActive(true);
                break;
            case (2):
                crabs[0].SetActive(true);
                crabs[1].SetActive(true);
                break;
            case (3):
                crabs[0].SetActive(true);
                crabs[1].SetActive(true);
                crabs[2].SetActive(true);
                break;
        }

    }
}
