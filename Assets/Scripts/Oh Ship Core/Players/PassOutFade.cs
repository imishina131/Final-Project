using System;
using UnityEngine;
using UnityEngine.UI;

public class PassOutFade : MonoBehaviour
{
    [SerializeField] Animator fadeAnim;
    [SerializeField] Image healthFill;
    [SerializeField] Image thirstFill;
    bool passedOut = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if((healthFill.fillAmount <= 0 || thirstFill.fillAmount <= 0) && !passedOut)
        {
            fadeAnim.SetBool("Fade", true);
            passedOut = true;
            Debug.Log("faded");
        }

        else if(healthFill.fillAmount > 0 && thirstFill.fillAmount > 0 && passedOut)
        {
            fadeAnim.SetBool("Fade", false);
            passedOut = false;
            Debug.Log("unfaded");
        }
    }
}
