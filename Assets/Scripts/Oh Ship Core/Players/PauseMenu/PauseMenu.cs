using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class PauseMenu : MonoBehaviour
{
    [FormerlySerializedAs("pauseMenuUI")] [SerializeField] GameObject m_pauseMenuUI;
    [FormerlySerializedAs("togglePauseAction")] [SerializeField] InputActionReference m_togglePauseAction;
    [FormerlySerializedAs("pauseMenuVolume")] [SerializeField] Volume m_pauseMenuVolume;
    [SerializeField] private GameObject m_firstSelectedButton;
    void Start()
    {
        m_pauseMenuUI.SetActive(false);
        foreach (PlayerInput playerInput in PlayerInput.all)
            SubscribeToPlayer(playerInput);
    }

    void OnEnable()
    {
        Debug.Log($"PauseMenu OnEnable, PlayerInputManager: {PlayerInputManager.instance}, PlayerInput.all count: {PlayerInput.all.Count}");

        if (PlayerInputManager.instance != null)
            PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        
    }

    void OnDisable()
    {
        if (PlayerInputManager.instance != null)
            PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
        foreach (PlayerInput playerInput in PlayerInput.all)
            UnsubscribeFromPlayer(playerInput);
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log($"OnPlayerJoined: {playerInput.name}");
        SubscribeToPlayer(playerInput);
    }

    void SubscribeToPlayer(PlayerInput playerInput)
    {
        InputAction pauseAction = playerInput.actions.FindAction("Pause");
        if (pauseAction == null) return;
        pauseAction.Enable();
        pauseAction.performed += OnToggleMenu;
    }

    void UnsubscribeFromPlayer(PlayerInput playerInput)
    {
        InputAction pauseAction = playerInput.actions.FindAction("Pause");
        if (pauseAction == null) return;
        pauseAction.performed -= OnToggleMenu;
    }

    public void Resume()
    {
        m_pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        m_pauseMenuVolume.weight = 0;
        foreach (PlayerController controller in FindObjectsByType<PlayerController>(FindObjectsSortMode.None))
            controller.TryChangeInputActionMap("Player", out _);
    }

    public void Pause()
    {
        m_pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        m_pauseMenuVolume.weight = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        foreach (PlayerController controller in FindObjectsOfType<PlayerController>())
            controller.TryChangeInputActionMap("UI", out _);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(m_firstSelectedButton);
    }

    void OnToggleMenu(InputAction.CallbackContext context)
    {
        if (!m_pauseMenuUI.activeSelf) Pause();
        else Resume();
    }
}
