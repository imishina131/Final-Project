using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public GameObject objectMusic;
    private AudioSource audio;

    private void Start()
    {
        objectMusic = GameObject.FindGameObjectWithTag("MenuMusic");
        audio = objectMusic.GetComponent<AudioSource>();
    }
}
