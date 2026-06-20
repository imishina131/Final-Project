using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HungerAndThirstVisualManager : MonoBehaviour
{
    [SerializeField] StatusBar m_hungerBar;
    [SerializeField] StatusBar m_thirstBar;
    [SerializeField] VolumeSettings m_hungerVolume;
    public void UpdateHunger(float hungerPercentage)
    {
        m_hungerBar.UpdateFillPercentage(hungerPercentage);
        m_hungerVolume.HandlePostEffects(hungerPercentage);
    }

    public void UpdateThirst(float thirstPercentage)
    {
        m_thirstBar.UpdateFillPercentage(thirstPercentage);
    }

    [Serializable]
    struct StatusBar
    {
        public Image Fill;
        public void UpdateFillPercentage(float fill) => Fill.fillAmount = fill;
    }
    
    [Serializable]
    struct VolumeSettings
    {
        public Volume Volume;
        public float FadePoint;
        public void HandlePostEffects(float hungerPercentage)
        {
            float fadeRange = 1f - FadePoint;
            float t = Mathf.Clamp01((FadePoint - hungerPercentage) / fadeRange);
            Volume.weight = t;
        }
    }
}
