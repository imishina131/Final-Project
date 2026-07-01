using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(RectTransform))]
public class PlayerSelectionCursor : MonoBehaviour
{
    [SerializeField] private Sprite[] playerIcons;
    RectTransform m_cursorTransform;
    MultiplayerEventSystem m_eventSystem;
    RectTransform m_selectedDestination;
    GameObject m_lastSelected;
    Vector3 m_velocity;
    private static int playerCount = 0;
    
    public int PlayerCount {get { return playerCount; } set { playerCount = value; } }
    void Awake()
    { 
        playerCount++;
        m_cursorTransform = GetComponent<RectTransform>();
        GetComponent<Image>().sprite = playerIcons[playerCount -1];
    }
    public void Initialize(MultiplayerEventSystem eventSystem) => m_eventSystem = eventSystem;
    void Update()
    {
        if (!m_eventSystem) return;
        if (m_eventSystem.currentSelectedGameObject && m_eventSystem.currentSelectedGameObject != m_lastSelected)
        {
            m_lastSelected = m_eventSystem.currentSelectedGameObject;
            m_selectedDestination = m_lastSelected.GetComponent<RectTransform>();
        }
        if (!m_selectedDestination) return;
        m_cursorTransform.position = Vector3.SmoothDamp(m_cursorTransform.position, new(m_selectedDestination.position.x, m_cursorTransform.position.y, m_selectedDestination.position.z), ref m_velocity, 0.1f);
    }
}