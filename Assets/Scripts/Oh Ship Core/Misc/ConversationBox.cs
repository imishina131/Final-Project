using UnityEngine;
using System.Collections;

public class ConversationBox : MonoBehaviour
{
    [Header("Voice lines, specifically has an enter and ext sound.")]
    [SerializeField] AudioClip m_enterSound;
    [Space]
    [SerializeField] AudioClip m_exitSound;
    [Space]
    [SerializeField] AudioClip beginningSound;
    [Space]
    [SerializeField] AudioClip randomSound01;
    [Space]
    [SerializeField] AudioClip randomSound02;
    [Space]
    [SerializeField] AudioClip randomSound03;
    [Space]
    [SerializeField] AudioClip randomSound04;
    private AudioSource m_audioSource;
    private string playerTag = "Player";
    private bool passed = false;
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
        if (other.CompareTag(playerTag) && !passed)
        {
            switch(other.tag)
            {
                case "Beginning":
                    m_enterSound = beginningSound;
                    break;
                case "Voiceline01":
                    m_enterSound = randomSound01;
                    break;
                case "Voiceline02":
                    m_enterSound = randomSound02;
                    break;
                case "Voiceline03":
                    m_enterSound = randomSound03;
                    break;
                case "Voiceline04":
                    m_enterSound = randomSound04;
                    break;
            }
            StartCoroutine(PlayAudio());
            passed = true;
        }
       
    }


    IEnumerator PlayAudio()
    {
        m_audioSource.PlayOneShot(m_enterSound);
        yield return new WaitForSeconds(m_enterSound.length);
        yield return new WaitForSeconds(1f);
        m_audioSource.PlayOneShot(m_exitSound);
    }
}
