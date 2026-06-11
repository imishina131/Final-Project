using UnityEngine;
using UnityEngine.Events;

public class ShipSinkCheck : MonoBehaviour
{
    [SerializeField] UnityEvent m_onSink;
    void FixedUpdate()
    {
        if (!(transform.position.y < -6)) return;
    }
}
