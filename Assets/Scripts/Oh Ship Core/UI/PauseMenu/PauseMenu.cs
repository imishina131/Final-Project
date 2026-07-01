using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class PauseMenu : MonoBehaviour
{
    [FormerlySerializedAs("pauseMenuUI")] [SerializeField] GameObject m_pauseMenuUI;
    [FormerlySerializedAs("togglePauseAction")] [SerializeField] InputActionReference m_togglePauseAction;
    [FormerlySerializedAs("pauseMenuVolume")] [SerializeField] Volume m_pauseMenuVolume;
    
    void Start()
    {
        m_pauseMenuUI.SetActive(false);
    }

    void OnEnable()
    {
        if (m_togglePauseAction == null) return;
        m_togglePauseAction.action.Enable();
        m_togglePauseAction.action.performed += OnToggleMenu;
    }

    void OnDisable()
    {
        if (m_togglePauseAction == null) return;
        m_togglePauseAction.action.performed -= OnToggleMenu;
        m_togglePauseAction.action.Disable();
    }

    public void Resume()
    {
        m_pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        m_pauseMenuVolume.weight = 0;
    }

    public void Pause()
    {
        m_pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        m_pauseMenuVolume.weight = 1;
    }
    
    void OnToggleMenu(InputAction.CallbackContext context)
    {
        if(!m_pauseMenuUI.activeSelf) Pause();
        else Resume();
    }
}
