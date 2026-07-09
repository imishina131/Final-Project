using UnityEngine;
using System.Collections;

public class ConversationBox : MonoBehaviour
{
    [Header("Voice lines, specifically has an enter and ext sound.")]
    [SerializeField] AudioClip m_enterSound;
    [Space]
    [SerializeField] AudioClip m_exitSound;
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
