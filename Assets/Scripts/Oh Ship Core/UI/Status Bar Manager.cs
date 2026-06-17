using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Playables;
using UnityEngine.UI;

public class StatusBarManager : MonoBehaviour
{
    [SerializeField] StatusBar m_hungerBar;
    [SerializeField] StatusBar m_thirstBar;

    [SerializeField] private Image fadeImage;
    private Color imageAlpha;
    public bool isPassedOut = false;
    public bool wokeUp = false;
    private GameObject m_player;
    public void InitializePlayerReference(GameObject player)
    {
        m_player = player;
    }

    public void UpdateHungerBar(float hungerPercentage)
    {
        m_hungerBar.UpdateFillPercentage(hungerPercentage);

        if (hungerPercentage <= 0f)
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
            Debug.Log("passes out");
            PassOut();
        }
        else if (hungerPercentage <= 0.3f && hungerPercentage > 0f)
        {
            Debug.Log("fades");
            SlowDown();
        }
    }
    public void UpdateThirstBar(float thirstPercentage) => m_thirstBar.UpdateFillPercentage(thirstPercentage);

    [Serializable]
    struct StatusBar
    {
        public Image Fill;
        public void UpdateFillPercentage(float fill)
        {
            Fill.fillAmount = fill;

        }
    }

    public void PassOut()
    {
        Debug.Log("passed: " + isPassedOut);
        isPassedOut = true;
        Debug.Log(m_player.name);
        m_player.layer = LayerMask.NameToLayer("Default");
        Debug.Log("passed: " + isPassedOut);
    }

    public void SlowDown()
    {
        imageAlpha = fadeImage.color;
        imageAlpha.a += Time.deltaTime/30;
        fadeImage.color = imageAlpha;
    }

    public void WakeUp()
    {
        wokeUp = true;
        isPassedOut = false;
        imageAlpha.a = 0;
        fadeImage.color = imageAlpha;
        m_player.layer = LayerMask.NameToLayer("Player");
    }

    public bool IsPassedOut()
    {
        return isPassedOut;
    }
        



}
