using System;
using TMPro;
using UnityEngine. UI;
using UnityEngine;
using UnityEngine.EventSystems;
public class HelpButtonSelection : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("Type what you want for the help text here")]
    [TextArea(3, 10)] [SerializeField] private string helpText;

    [Header("Insert SFX here to play")]
    [SerializeField] private AudioClip hoverSFX;
    
    private GameObject m_helpPictureLabel;
    [SerializeField] AudioSource m_audioSource;
    
    public Sprite _sprite;

    private void Start()
    {
        
        m_helpPictureLabel = transform.GetChild(0).gameObject;
        m_helpPictureLabel.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (m_helpPictureLabel == null) return;
        m_audioSource.PlayOneShot(hoverSFX);
        m_helpPictureLabel.SetActive(true);

    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (m_helpPictureLabel == null) return;
        m_helpPictureLabel.SetActive(false);
    }

    public void OnClick()
    {
        HelpMenuManager.Instance.ShowHelp(helpText, _sprite);
    }
}

