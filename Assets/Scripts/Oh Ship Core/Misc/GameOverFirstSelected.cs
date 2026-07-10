using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameOverFirstSelected : MonoBehaviour
{
    [SerializeField] private GameObject m_firstSelected;
    void Start()
    {
        
        StartCoroutine(SelectFirstAsync());
    }

    IEnumerator SelectFirstAsync()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(m_firstSelected);
        Debug.Log($"Set selected to: {EventSystem.current.currentSelectedGameObject}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
