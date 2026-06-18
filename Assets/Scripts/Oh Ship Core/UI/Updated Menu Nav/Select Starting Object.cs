using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;

public class SelectStartingObject : MonoBehaviour
{
    [FormerlySerializedAs("firstSelectedObject")] [SerializeField] GameObject m_firstSelectedObject;
    [SerializeField] Canvas m_canvas;
    public void OnSpawnPlayer(PlayerInput playerInput)
    {
        StartCoroutine(SelectFirstAsync(playerInput));
    }

    IEnumerator SelectFirstAsync(PlayerInput playerInput)
    {
        MultiplayerEventSystem eventSystem = playerInput.gameObject.GetComponentInChildren<MultiplayerEventSystem>();
        Debug.Log($"EventSystem found: {eventSystem}, firstSelected: {m_firstSelectedObject}");
        eventSystem.firstSelectedGameObject = m_firstSelectedObject;
        eventSystem.playerRoot = m_canvas.gameObject;
        yield return null;
        eventSystem.SetSelectedGameObject(m_firstSelectedObject);
    }
}
