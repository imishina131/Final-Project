using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FishingManager : MonoBehaviour, IInteractable
{
    [SerializeField] private RectTransform greenZone;
    [SerializeField] private RectTransform playerFishingIcon;
    [SerializeField] private Slider fishingProgressBar;
    [SerializeField] private RectTransform usableFishingArea;
    [SerializeField] private float speedOfFishIcon;
    
    [Range(0f, 1f)]
    [SerializeField] private float progressSpeed;
    
    private bool _isHoldingButton = false;
    private float _minOfUsableFishingSpace;
    private float _maxOfUsableFishingSpace;
    
    private float _halfHeightOfFish;
    
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        Debug.Log("Beginning Interaction");
        (_minOfUsableFishingSpace,_maxOfUsableFishingSpace) = GetMaxAndMinOfIcon(usableFishingArea);
      var (bottomOfFishIcon, topOfFishIcon) = GetMaxAndMinOfIcon(playerFishingIcon);
         _halfHeightOfFish = (topOfFishIcon - bottomOfFishIcon) / 2;
    
        Vector3 startPos = playerFishingIcon.position;
        startPos.y = _minOfUsableFishingSpace + _halfHeightOfFish;
        playerFishingIcon.position = startPos;
        
       return  new InteractionSession(interactor, this);
    }

    public void EndInteraction(InteractionSession session)
    {
        Debug.Log("End Interaction");
    }

    private void Update()
    {
        _isHoldingButton = Keyboard.current.spaceKey.isPressed;
        StartFishing(playerFishingIcon, usableFishingArea);
        CheckFishingProgress(fishingProgressBar);
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


    private void StartFishing(RectTransform incomingFishingIcon, RectTransform incomingUsableFishingSpace)
    {
        var (bottomOfFishArea, topOfFishArea) = GetMaxAndMinOfIcon(incomingUsableFishingSpace);
        
        float directionOfFish = _isHoldingButton ? speedOfFishIcon : -speedOfFishIcon;
        
        Vector3 worldPos = incomingFishingIcon.position;
        worldPos.y += directionOfFish * Time.deltaTime;
        worldPos.y = Mathf.Clamp(worldPos.y, 
            bottomOfFishArea + _halfHeightOfFish, 
            topOfFishArea - _halfHeightOfFish);
        
        incomingFishingIcon.position = worldPos;
    }
    
}
