using UnityEngine;
using UnityEngine.Events;

public class EndGame : MonoBehaviour
{
    public UnityEvent endEvent;
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player Steam Boat"))
        {
            Debug.Log("New Scene");
            endEvent.Invoke();
        }
    }
}
