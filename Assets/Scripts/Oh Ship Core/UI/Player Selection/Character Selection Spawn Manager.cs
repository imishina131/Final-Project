using System.Collections;
using MatrixUtils.Attributes;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
public class CharacterSelectionSpawnManager : MonoBehaviour
{
    [Inject] IInjector m_injector;
    [SerializeField, RequiredField] PlayerInputManager m_playerInputManager;
    [SerializeField, RequiredField] Canvas m_playerSelectionCanvasPrefab;
    [SerializeField, RequiredField] Canvas m_displayCanvas;
    [SerializeField, RequiredField] RectTransform m_playerSelectionLayout;
    [SerializeField, RequiredField] PlayerSelectionCursor m_playerSelectionCursorPrefab;
    public void OnSpawn(PlayerInput playerInput)
    {
        if(playerInput.transform.root.GetComponentInChildren<MultiplayerEventSystem>() is not {} multiplayerEventSystem) return;
        StartCoroutine(SpawnPlayerSelectionCanvas(multiplayerEventSystem));
    }

    IEnumerator SpawnPlayerSelectionCanvas(MultiplayerEventSystem multiplayerEventSystem)
    {
        yield return null;
        Canvas selectionCanvas = Instantiate(m_playerSelectionCanvasPrefab);
        m_injector.Inject(selectionCanvas.gameObject);
        multiplayerEventSystem.playerRoot = selectionCanvas.gameObject;
        if (!TryGetChildWithTag(selectionCanvas.transform, "Multiplayer UI Start Position", out Transform startPosition)) yield break;
        multiplayerEventSystem.SetSelectedGameObject(startPosition.gameObject);
        multiplayerEventSystem.firstSelectedGameObject = startPosition.gameObject;
        PlayerSelectionCursor cursor = Instantiate(m_playerSelectionCursorPrefab, m_playerSelectionLayout);
        cursor.Initialize(multiplayerEventSystem);
    }

    static bool TryGetChildWithTag(Transform transform, string tag, out Transform result)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.CompareTag(tag))
            {
                result = child;
                return true;
            }
            if (TryGetChildWithTag(child, tag, out result)) return true;
        }
        result = null;
        return false;
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded");
        m_playerSelectionCursorPrefab.PlayerCount = 0;
    }

}