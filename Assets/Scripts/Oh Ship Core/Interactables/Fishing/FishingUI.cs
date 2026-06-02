using UnityEngine;
using UnityEngine.UI;

public class FishingUI : MonoBehaviour
{
    [SerializeField] private RectTransform greenZone;
    [SerializeField] private RectTransform playerFishingIcon;
    [SerializeField] private RectTransform usableFishingArea;
    [SerializeField] private Slider fishingProgress;
    [SerializeField] private GameObject fishingUI;
    
    public RectTransform GreenZone => greenZone;
    public RectTransform PlayerFishingIcon => playerFishingIcon;
    public RectTransform UsableFishingArea => usableFishingArea;
    public Slider FishingProgressBar => fishingProgress;
    
    public void DisplayFishingUI()
    {
        fishingUI.SetActive(true);
    }

    public void HideFishingUI()
    {
        fishingUI.SetActive(false);
    }
}