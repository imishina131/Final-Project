using UnityEngine;
using UnityEngine.SceneManagement;
public class DontDestroy : MonoBehaviour
{
    public string[] scenesToCheck;
    AudioSource audio;
    private void Awake()
    {
        GameObject[] musicObj = GameObject.FindGameObjectsWithTag("MenuMusic");
        if(musicObj.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        audio = this.gameObject.GetComponent<AudioSource>();


    }

    private void Update()
    {
        if (isCurrentSceneInArray(scenesToCheck))
        {
            if(!audio.isPlaying)
            {
                audio.Play();
            }
            else
            {
                return;
            }
        }
        else
        {
            audio.Pause();
        }
    }

    bool isCurrentSceneInArray(string[] sceneNames)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        foreach (string scene in sceneNames)
        {
            if (currentSceneName == scene)
            {
                return true; 
            }
        }

        return false; 
    }
}
