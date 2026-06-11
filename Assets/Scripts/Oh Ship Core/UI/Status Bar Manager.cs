using System;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarManager : MonoBehaviour
{
    [SerializeField] StatusBar m_hungerBar;
    [SerializeField] StatusBar m_thirstBar;
    public void UpdateHungerBar(float hungerPercentage) => m_hungerBar.UpdateFillPercentage(hungerPercentage);
    public void UpdateThirstBar(float thirstPercentage) => m_thirstBar.UpdateFillPercentage(thirstPercentage);
    
    [Serializable]
    struct StatusBar
    {
        public Image Fill;
        public void UpdateFillPercentage(float fill)
        {
            //Debug.Log("fill");
            //Fill.fillAmount = Mathf.Clamp01(fill);
            Fill.fillAmount = fill;
        }
    }
}
