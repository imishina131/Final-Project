using System;
using System.Collections;
using UnityEngine;

public class PoliceBridge : MonoBehaviour
{
    [Header("Police voice lines, specifically only has an enter and ext sound.")]
    [SerializeField] AudioClip m_policeEnterSound;
    [Space]
    [SerializeField] AudioClip m_policeExitSound;
    
    [Header("Make sure this is the tag of the ship not the player")]
    [SerializeField] private string playerShipTag;
    
    [Header("Police boats to spawn in the scene")]
    [SerializeField] private GameObject policeBoats;
    
    private AudioSource m_audioSource;
    private bool passedBridge = false;
    
    
    void Start()
    {
        m_audioSource = GetComponentInParent<AudioSource>();
        if (m_audioSource == null)
        {
            Debug.Log("AudioSource is null");
        }
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerShipTag) && !passedBridge)
        {
            StartCoroutine(PlayAudio());
            passedBridge = true;
        }
       
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerShipTag) && !policeBoats.activeSelf)
        {
            policeBoats.SetActive(true);
        }
    }

    IEnumerator PlayAudio()
    {
        m_audioSource.PlayOneShot(m_policeEnterSound);
        yield return new WaitForSeconds(m_policeEnterSound.length);
        yield return new WaitForSeconds(1.5f);
        m_audioSource.PlayOneShot(m_policeExitSound);
    }

    
}
