using System;
using UnityEngine;

using UnityEngine.UI;

public class FishingMiniGame
{
    private FishingMiniGameData _data;
    private float _halfHeightOfFishingIcon;

    public Action OnCaughtFish;
    
    private bool _isGameOver;
   
    public void InitializeMiniGame(FishingMiniGameData fishingMiniGameData)
    {
        _isGameOver = false;
        _data = fishingMiniGameData;
        _data.FishingProgressBar.value = 0;
        var (bottomOfFishIcon, topOfFishIcon) = GetMaxAndMinOfIconWorld(_data.PlayerFishingIcon);
        _halfHeightOfFishingIcon = (topOfFishIcon - bottomOfFishIcon) / 2;
        
        var (minOfUsableFishingSpace,maxOfUsableFishingSpace) = GetMaxAndMinOfIconWorld(_data.UsableFishingArea);
        Vector3 startPos = _data.PlayerFishingIcon.position;
        startPos.y = minOfUsableFishingSpace + _halfHeightOfFishingIcon;
        _data.PlayerFishingIcon.position = startPos;
        CheckFishingProgress(_data.FishingProgressBar);
        
        
    }
    
    public void UpdateMiniGame(bool isHoldingButton)
    {
        PlayingMinigame(isHoldingButton);
        CheckFishingProgress(_data.FishingProgressBar);
    }

    public void EndMiniGame()
    {
        _isGameOver = true;
    }

    public void PlayingMinigame(bool isHoldingButton)
    {
        if(_isGameOver) return;
        var (bottomOfFishArea, topOfFishArea) = GetMaxAndMinOfIconLocal(_data.UsableFishingArea);
        float directionOfFish = isHoldingButton ? _data.SpeedOfFishIcon : -_data.SpeedOfFishIcon;
        Vector3 localPos = _data.PlayerFishingIcon.localPosition;
        localPos.y += directionOfFish * Time.deltaTime;       
       
        localPos.y = Mathf.Clamp(localPos.y, 
            bottomOfFishArea+ _halfHeightOfFishingIcon, 
            topOfFishArea - _halfHeightOfFishingIcon);
        
        _data.PlayerFishingIcon.localPosition = localPos;
    }

    

    private void CheckFishingProgress(Slider incomingSlider)
    {
        if(_isGameOver) return;
        
        if (FishingIconOverlap(_data.PlayerFishingIcon, _data.GreenZone))
        {
            incomingSlider.value += _data.ProgressSpeed * Time.deltaTime;
        }
        else
        {
            incomingSlider.value -= _data.ProgressSpeed * Time.deltaTime;
        }
        incomingSlider.value = Mathf.Clamp(incomingSlider.value, 0f, 1f);
        
        if (incomingSlider.value >= incomingSlider.maxValue)
        {
            OnCaughtFish?.Invoke();
            EndMiniGame();
        }
        
    }
    
    private bool FishingIconOverlap(RectTransform fishIcon, RectTransform greenZone)
    {
        var (bottomOfFishIcon, topOfFishIcon) = GetMaxAndMinOfIconWorld(fishIcon);
        var (bottomOfGreenZoneIcon, topOfGreenZoneIcon) = GetMaxAndMinOfIconWorld(greenZone);
        
        if ((bottomOfGreenZoneIcon <= bottomOfFishIcon && topOfFishIcon <= topOfGreenZoneIcon))
        {
            return true;
        }
        return false;
    }

    private (float min, float max) GetMaxAndMinOfIconWorld(RectTransform incomingIcon)
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
}
