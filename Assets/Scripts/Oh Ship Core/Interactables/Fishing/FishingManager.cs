using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;

public class FishingManager : MonoBehaviour, IInteractable, IPlayerControllable
{
    [SerializeField] private string _fishingControlActionMap = "Fishing";
    private RectTransform greenZone;
    private RectTransform playerFishingIcon;
    private Slider fishingProgressBar;
    private RectTransform usableFishingArea;
    [SerializeField] private float speedOfFishIcon;
    
    [Range(0f, 1f)]
    [SerializeField] private float progressSpeed;
    
    private bool _isHoldingButton = false;
    private float _minOfUsableFishingSpace;
    private float _maxOfUsableFishingSpace;
    private float _halfHeightOfFish;
    private FishingUI _fishingUI;
    private InputActionMap _activeActionMap;
    private InteractionSession _currentInteractionSession;
    private IPlayerController _playerController;
    private IPlayerControllable _playerControllable;
    
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        Debug.Log("Beginning Interaction");

        _playerControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        _playerController = _playerControllable.GetActivePlayerController();
        

        SetUpFishingMinigame(interactor);
        _playerController.ChangeControlledEntity(this);

        _currentInteractionSession = new InteractionSession(interactor, this);
        _currentInteractionSession.OnEnded += () => _playerController.ChangeControlledEntity(_playerControllable);
        
       
        
        return _currentInteractionSession;
    }
    

    private bool FishingIconOverlap(RectTransform fishIcon, RectTransform greenZone)
    {
        var (bottomOfFishIcon, topOfFishIcon) = GetMaxAndMinOfIcon(fishIcon);
        var (bottomOfGreenZoneIcon, topOfGreenZoneIcon) = GetMaxAndMinOfIcon(greenZone);
        
        if ((bottomOfGreenZoneIcon <= bottomOfFishIcon && topOfFishIcon <= topOfGreenZoneIcon))
        {
            Debug.Log("It's on!");
            return true;
        }
        return false;
    }

    private (float min, float max) GetMaxAndMinOfIcon(RectTransform incomingIcon)
    {
        Vector3[] worldCorners = new Vector3[4];
        incomingIcon.GetWorldCorners(worldCorners);

        return (worldCorners[0].y, worldCorners[1].y);
    }
    
    private (float min, float max) GetMaxAndMinOfIconLocal(RectTransform incomingIcon)
    {
        Vector3[] worldCorners = new Vector3[4];
        incomingIcon.GetLocalCorners(worldCorners);

        return (worldCorners[0].y, worldCorners[1].y);
    }


    private void Update()
    {
        if(_playerController == null) return;
        StartFishing();
        CheckFishingProgress(fishingProgressBar);
    }

    private void CheckFishingProgress(Slider incomingSlider)
    {

        if (FishingIconOverlap(playerFishingIcon, greenZone))
        {
            incomingSlider.value += progressSpeed * Time.deltaTime;
        }
        else
        {
            incomingSlider.value -= progressSpeed * Time.deltaTime;
        }
        incomingSlider.value = Mathf.Clamp(incomingSlider.value, 0f, 1f);
        
    }

    private void HandleInteract(InputAction.CallbackContext context) => _currentInteractionSession.End();


    private void HandleFishingInput(InputAction.CallbackContext context)
    {
        _isHoldingButton = context.performed;
    }
    private void StartFishing()
    {
        var (bottomOfFishArea, topOfFishArea) = GetMaxAndMinOfIconLocal(usableFishingArea);
        float directionOfFish = _isHoldingButton ? speedOfFishIcon : -speedOfFishIcon;
        Vector3 localPos = playerFishingIcon.localPosition;
        localPos.y += directionOfFish * Time.deltaTime;       
       
        localPos.y = Mathf.Clamp(localPos.y, 
             bottomOfFishArea+ _halfHeightOfFish, 
            topOfFishArea - _halfHeightOfFish);
        
        playerFishingIcon.localPosition = localPos;
    }

    private void SetUpUIElements(IInteractor interactor)
    {

        GameObject player = _playerController.GetAssociatedGameObject();

        _fishingUI = player.GetComponentInChildren<FishingUI>();
        
        greenZone = _fishingUI.GreenZone;
        playerFishingIcon = _fishingUI.PlayerFishingIcon;
        fishingProgressBar = _fishingUI.FishingProgressBar;
        usableFishingArea = _fishingUI.UsableFishingArea;
        
        _fishingUI.DisplayFishingUI();
    }

    private void SetUpFishingMinigame(IInteractor interactor)
    {
        SetUpUIElements(interactor);
        Canvas.ForceUpdateCanvases();
        
        (_minOfUsableFishingSpace,_maxOfUsableFishingSpace) = GetMaxAndMinOfIcon(usableFishingArea);
        var (bottomOfFishIcon, topOfFishIcon) = GetMaxAndMinOfIcon(playerFishingIcon);
        _halfHeightOfFish = (topOfFishIcon - bottomOfFishIcon) / 2;
        
        
    
        Vector3 startPos = playerFishingIcon.position;
        startPos.y = _minOfUsableFishingSpace + _halfHeightOfFish;
        playerFishingIcon.position = startPos;
        CheckFishingProgress(fishingProgressBar);
       
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
        _fishingUI.HideFishingUI();
        _activeActionMap = null;
    }

    public IPlayerController GetActivePlayerController() => _playerController;
    

    public GameObject GetAssociatedGameObject()
    {
        return gameObject;
    }
}
