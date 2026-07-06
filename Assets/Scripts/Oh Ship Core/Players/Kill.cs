using UnityEngine;
using UnityEngine.SceneManagement;

public class Kill : MonoBehaviour
{ 

    [SerializeField] ISceneTransitioner sceneTransitioner;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Kill")
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
