using System;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using Random = System.Random;

public class FishingManager : MonoBehaviour, IInteractable, IPlayerControllable, IPromptProvider
{
    [SerializeField] private string _widgetForPrompt = "interact";
    [SerializeField] private string _fishingControlActionMap = "Fishing";
    
    [SerializeField] private float _speedOfFishIcon;
    
    [Range(0f, 1f)]
    [SerializeField] private float _progressSpeed;

    [Header("Add fish models here!")] 
    [SerializeField]
    private GameObject[] usableFishesToCatch;
    
    private bool _isHoldingButton = false;

    private FishingUI _fishingUI;
    private InputActionMap _activeActionMap;
    private InteractionSession _currentInteractionSession;
    private IPlayerController _playerController;
    private IPlayerControllable _playerControllable;
    private FishingMiniGame _fishingMiniGame;
    private RectTransform _greenZone;
    private RectTransform _playerFishingIcon;
    private Slider _fishingProgressBar;
    private RectTransform _usableFishingArea;
    private Transform _holdingObjectTransform;
    
    private void Start()
    {
        _fishingMiniGame = new FishingMiniGame();
    }
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        Debug.Log("Beginning Interaction");

        if (interactor.IsHoldingObject())
        {
             Debug.Log("Holding Fish");
            _currentInteractionSession = new InteractionSession(interactor, this);
            _currentInteractionSession.End();
            return _currentInteractionSession;
        }

        _playerControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        _playerController = _playerControllable.GetActivePlayerController();
        
        SetUpFishingMinigame();
        _playerController.ChangeControlledEntity(this);

        _currentInteractionSession = new InteractionSession(interactor, this);
        _currentInteractionSession.OnEnded += () => _playerController.ChangeControlledEntity(_playerControllable);
        return _currentInteractionSession;
    }
 
    private void Update()
    {
        if(_playerController == null) return;
        _fishingMiniGame.UpdateMiniGame(_isHoldingButton);
       
    }

    private void HandleInteract(InputAction.CallbackContext context) => _currentInteractionSession.End();


    private void HandleFishingInput(InputAction.CallbackContext context)
    {
        _isHoldingButton = context.performed;
    }

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
        _fishingMiniGame.OnCaughtFish += HandleFishCaught;
    }

    public void OnControlRequested(IPlayerController player)
    {
        _playerController = player;
        
        if (!_playerController.ChangeInputActionMap(_fishingControlActionMap, out InputActionMap map))
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
        InputAction reelFishAction = _activeActionMap.FindAction("Reel Fish");
        reelFishAction.performed -= HandleFishingInput;
        reelFishAction.canceled -= HandleFishingInput;
        
        InputAction interactAction = _activeActionMap.FindAction("Interact");
        interactAction.performed -= HandleInteract;
        
        _fishingMiniGame.OnCaughtFish -= HandleFishCaught;
        _fishingUI.HideFishingUI();
        _activeActionMap = null;
    }

    public IPlayerController GetActivePlayerController() => _playerController;

    private void HandleFishCaught()
    {
        Debug.Log("Fish Caught");
        int index = UnityEngine.Random.Range(0, usableFishesToCatch.Length);
        GameObject player = _playerControllable.GetAssociatedGameObject();

        _holdingObjectTransform = player.GetComponentInChildren<HeldObjectLocation>().transform;
        
        GameObject caughtFish = Instantiate(usableFishesToCatch[index], _holdingObjectTransform.position,_holdingObjectTransform.rotation);
        caughtFish.transform.SetParent(_holdingObjectTransform);
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
        return transform.position;
    }
}
