using System;
using System.Data;
using System.Collections;
using MatrixUtils.DependencyInjection;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using Random = System.Random;

public class FishingManager : MonoBehaviour, IInteractable, IPlayerControllable, IPromptProvider
{
    [SerializeField] private CinemachineCamera _fishingCamera;
    [SerializeField] private string _widgetForPrompt = "interact";
    [SerializeField] private string _fishingControlActionMap = "Fishing";
    [SerializeField] Transform m_widgetPosition;

    [SerializeField] private float _speedOfFishIcon;

    
    [SerializeField] private float _overlapIconBuffer = 0.9f;
        
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
    private IPlayerControllable _playerControllableForHoldingObject;
    private FishingMiniGame _fishingMiniGame;
    private RectTransform _greenPlayerZone;
    private RectTransform _fishingIcon;
    private Image _fishingProgressBar;
    private RectTransform _usableFishingArea;
    private Transform _holdingObjectTransform;
    private IInteractor _interactor;
    private GameObject _player; //Used to disable model renderer
    private PlayerInteractionState _playerInteractionState;
    [SerializeField] private float _minFishSpeed;
    [SerializeField] private float _maxFishSpeed;

    [Header("Audio")]
    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip plop;
    [SerializeField] AudioClip fishing;

    private void Start()
    {
        _fishingMiniGame = new FishingMiniGame();
        audio = GetComponent<AudioSource>();
    }
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        Debug.Log("Beginning Interaction");
        IPlayerControllable oldControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        IPlayerController controller = oldControllable.GetActivePlayerController();
        _playerControllableForHoldingObject = oldControllable;
        _playerController = controller;
        _playerInteractionState = oldControllable.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();

        if (_playerInteractionState.CheckInteractionTag(InteractionTag.HoldingFish) || _playerInteractionState.CheckInteractionTag(InteractionTag.HoldingBottle) || _playerInteractionState.CheckInteractionTag(InteractionTag.HoldingBottleWithWater) || _currentInteractionSession is { IsActive: true })
        {
            Debug.Log("Blocked");
            return null;
        }

        
        CinemachineCamera playerCam = interactor.GetAssociatedGameObject().GetComponent<CinemachineCamera>();
        audio.clip = plop;
        audio.PlayOneShot(plop);
        _fishingCamera.OutputChannel = playerCam.OutputChannel;
        _fishingCamera.Priority = 10;
        SetUpFishingMinigame();
        controller.ChangeControlledEntity(this);

        _currentInteractionSession = new InteractionSession(interactor, this);
        _currentInteractionSession.OnEnded += () => controller.ChangeControlledEntity(oldControllable);
        _interactor  = interactor;
        _player = oldControllable.GetAssociatedGameObject().gameObject;
        _player.GetComponentInChildren<MeshRenderer>().enabled = false;
        
        _playerInteractionState.AddInteractionTag(InteractionTag.Fishing);
        return _currentInteractionSession;
    }
 
    private void Update()
    {
        if(_playerController == null) return;
        _fishingMiniGame.UpdateMiniGame(_isHoldingButton);
        if(_isHoldingButton && !audio.isPlaying)
        {
            audio.clip = fishing;
            audio.loop = true;
            audio.Play();
        }
        else if(!_isHoldingButton && audio.loop == true)
        {
            audio.loop = false;
            audio.Stop();
        }
       
    }

    private void HandleInteract(InputAction.CallbackContext context) => _currentInteractionSession.End();
    
    private void HandleFishingInput(InputAction.CallbackContext context) => _isHoldingButton = context.performed;

    private void SetUpUIElements()
    {
        GameObject player = _playerController.GetAssociatedGameObject();

        _fishingUI = player.GetComponentInChildren<FishingUI>();
        
        _greenPlayerZone = _fishingUI.PlayerGreenZone;
        _fishingIcon = _fishingUI.FishingIcon;
        _fishingProgressBar = _fishingUI.FishingProgressBar;
        _usableFishingArea = _fishingUI.UsableFishingArea;
        
        _fishingUI.DisplayFishingUI();
    }

    private  void SetUpFishingMinigame()
    {
        _isHoldingButton = false;
        SetUpUIElements();
    
        FishingMiniGameData data = new FishingMiniGameData();
        data.PlayerGreenZone = _greenPlayerZone;
        data.UsableFishingArea = _usableFishingArea;
        data.FishingIcon = _fishingIcon;
        data.FishingProgressBar = _fishingProgressBar;
        data.SpeedOfFishIcon = _speedOfFishIcon;
        data.ProgressSpeed = _progressSpeed;
        data.IconBuffer = _overlapIconBuffer;
        data.FishMinSpeed = _minFishSpeed * 10;
        data.FishMaxSpeed = _maxFishSpeed * 10;
        
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
        _fishingMiniGame.EndMiniGame();
        _playerInteractionState.RemoveInteractionTag(InteractionTag.Fishing);
        _activeActionMap = null;
    }

    public IPlayerController GetActivePlayerController() => _playerController;

    private void HandleObjectCaught()
    {
        FoodClass foodClassRef;
        _isHoldingButton = false;
        Debug.Log("Fish Caught");
        int index = UnityEngine.Random.Range(0, usableThingsToCatch.Length);
        
        _holdingObjectTransform = _playerControllableForHoldingObject.GetAssociatedGameObject().GetComponentInChildren<HeldObjectHandler>().transform;
        GameObject caughtItem = Instantiate(usableThingsToCatch[index], _holdingObjectTransform.position,_holdingObjectTransform.rotation);
        caughtItem.transform.SetParent(_holdingObjectTransform);
        if (caughtItem.GetComponentInChildren<Fish>())
        {
            caughtItem.transform.localRotation = Quaternion.Euler(44.1f, 142f, 77.5f);
        }
        _playerInteractionState.AddInteractionTag(InteractionTag.HoldingFish);
       // _playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingCookedFish);

        foodClassRef = caughtItem.GetComponent<FoodClass>();
        HungerAndThirst hungerRef = _playerControllableForHoldingObject.GetAssociatedGameObject().GetComponentInChildren<HungerAndThirst>();
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
