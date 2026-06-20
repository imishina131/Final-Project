using System;
using System.Data;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using Random = System.Random;

public class FishingManager : MonoBehaviour, IInteractable, IPlayerControllable, IPromptProvider
{
    [SerializeField] private StorageInteractable _fishBoxStorage;
    [SerializeField] private CinemachineCamera _fishingCamera;
    [SerializeField] private string _widgetForPrompt = "interact";
    [SerializeField] private string _fishingControlActionMap = "Fishing";
    [SerializeField] Transform m_widgetPosition;

    [SerializeField] private float _speedOfFishIcon;
    
    [Range(0f, 1f)]
    [SerializeField] private float _progressSpeed;

    [Header("Add edible catchable models here!")] 
    [SerializeField]
    private GameObject[] usableThingsToCatch;
    
    private bool _isHoldingButton = false;

    private FishingUI _fishingUI;
    private InputActionMap _activeActionMap;
    private InteractionSession _currentInteractionSession;
    private IPlayerController _playerController;
    private IPlayerControllable _playerControllable;
    private FishingMiniGame _fishingMiniGame;
    private RectTransform _greenZone;
    private RectTransform _playerFishingIcon;
    private Image _fishingProgressBar;
    private RectTransform _usableFishingArea;
    private Transform _holdingObjectTransform;
    private IInteractor _interactor;
    private GameObject _player; //Used to disable model renderer
    private PlayerInteractionState _playerInteractionState;
    
    private void Start()
    {
        _fishingMiniGame = new FishingMiniGame();
    }
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        Debug.Log("Beginning Interaction");
        _playerControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        _playerController = _playerControllable.GetActivePlayerController();
        _playerInteractionState = _playerControllable.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();
       
        if (_playerInteractionState.CheckInteractionTag(InteractionTag.Holding))
        {
             Debug.Log("Holding Fish");
            _currentInteractionSession = new InteractionSession(interactor, this);
            _currentInteractionSession.End();
            return _currentInteractionSession;
        }

        
        CinemachineCamera playerCam = interactor.GetAssociatedGameObject().GetComponent<CinemachineCamera>();
        _fishingCamera.OutputChannel = playerCam.OutputChannel;
        _fishingCamera.Priority = 10;
        SetUpFishingMinigame();
        _playerController.ChangeControlledEntity(this);

        _currentInteractionSession = new InteractionSession(interactor, this);
        _currentInteractionSession.OnEnded += () => _playerController.ChangeControlledEntity(_playerControllable);
        _interactor  = interactor;
        _player = _playerControllable.GetAssociatedGameObject().gameObject;
        _player.GetComponentInChildren<MeshRenderer>().enabled = false;
        
       
        return _currentInteractionSession;
    }
 
    private void Update()
    {
        if(_playerController == null) return;
        _fishingMiniGame.UpdateMiniGame(_isHoldingButton);
       
    }

    private void HandleInteract(InputAction.CallbackContext context) => _currentInteractionSession.End();
    
    private void HandleFishingInput(InputAction.CallbackContext context) => _isHoldingButton = context.performed;

    private void SetUpUIElements()
    {
        GameObject player = _playerController.GetAssociatedGameObject();

        _fishingUI = player.GetComponentInChildren<FishingUI>();
        
        _greenZone = _fishingUI.GreenZone;
        _playerFishingIcon = _fishingUI.PlayerFishingIcon;
        _fishingProgressBar = _fishingUI.FishingProgressBar;
        _usableFishingArea = _fishingUI.UsableFishingArea;
        
        _fishingUI.DisplayFishingUI();
    }

    private void SetUpFishingMinigame()
    {
        _isHoldingButton = false;
        SetUpUIElements();
        
        FishingMiniGameData data = new FishingMiniGameData();
        data.PlayerFishingIcon = _playerFishingIcon;
        data.UsableFishingArea = _usableFishingArea;
        data.GreenZone =  _greenZone;
        data.FishingProgressBar = _fishingProgressBar;
        data.SpeedOfFishIcon = _speedOfFishIcon;
        data.ProgressSpeed = _progressSpeed;
        
        _fishingMiniGame.InitializeMiniGame(data);
        _fishingMiniGame.OnCaughtFish += HandleObjectCaught;
    }

    public void OnControlRequested(IPlayerController player)
    {
        _playerController = player;
        
        if (!_playerController.TryChangeInputActionMap(_fishingControlActionMap, out InputActionMap map))
        {
            Debug.LogError("Failed to assign input actions to player, reverting control to default.");
            _playerController.ChangeControlledEntity(null);
            return;
        }
        _activeActionMap = map;
        InputAction reelFishAction = _activeActionMap.FindAction("Reel Fish");
        reelFishAction.performed += HandleFishingInput;
        reelFishAction.canceled += HandleFishingInput;
        
        InputAction interactAction = _activeActionMap.FindAction("Interact");
        interactAction.performed += HandleInteract;
    }

    public void OnControlReleased()
    {
        _fishingCamera.Priority = 0;
        InputAction reelFishAction = _activeActionMap.FindAction("Reel Fish");
        reelFishAction.performed -= HandleFishingInput;
        reelFishAction.canceled -= HandleFishingInput;
        
        InputAction interactAction = _activeActionMap.FindAction("Interact");
        interactAction.performed -= HandleInteract;
        
        _fishingMiniGame.OnCaughtFish -= HandleObjectCaught;
        _fishingUI.HideFishingUI();
        _player.GetComponentInChildren<MeshRenderer>().enabled = true;
        _activeActionMap = null;
    }

    public IPlayerController GetActivePlayerController() => _playerController;

    private void HandleObjectCaught()
    {
        FoodClass foodClassRef;
        Debug.Log("Fish Caught");
        int index = UnityEngine.Random.Range(0, usableThingsToCatch.Length);
        
        _holdingObjectTransform = _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectLocation>().transform;
        GameObject caughtItem = Instantiate(usableThingsToCatch[index], _holdingObjectTransform.position,_holdingObjectTransform.rotation);
        caughtItem.transform.SetParent(_holdingObjectTransform);
        _playerInteractionState.AddInteractionTag(InteractionTag.Holding);
        
        foodClassRef = caughtItem.GetComponent<FoodClass>();
        HungerAndThirst hungerRef = _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HungerAndThirst>();
        Debug.Log($"HungerAndThirst found: {hungerRef}");
        foodClassRef.InitializeHungerAndThirst(hungerRef);      
        _currentInteractionSession.End();
    }

    public GameObject GetAssociatedGameObject()
    {
        return gameObject;
    }

    public PromptData GetPromptData()
    {
        return new PromptData() { AssociatedWidget = _widgetForPrompt, };
    }

    public Vector3 GetWidgetWorldPosition()
    {
        return m_widgetPosition.position;
    }
}
