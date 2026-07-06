using UnityEngine;
//using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using System.Collections.Generic;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
   
    [FormerlySerializedAs("pauseMenuUI")] [SerializeField] GameObject m_pauseMenuUI;
    [FormerlySerializedAs("pauseMenuVolume")] [SerializeField] Volume m_pauseMenuVolume;
    [SerializeField] private PauseMenuNavigation m_pauseMenuNavigation;
    private float m_lastMovement;
    private float  m_movementCoolDown = .15f;
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
        Debug.Log("Disable");
        if (PlayerInputManager.instance != null)
            PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
        
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log($"OnPlayerJoined: {playerInput.name}");
        SubscribeToPlayer(playerInput);
    }

    void SubscribeToPlayer(PlayerInput playerInput)
    {
        InputAction pauseAction = playerInput.actions.FindAction("Pause");
        if (pauseAction != null) { pauseAction.Enable(); pauseAction.performed += OnToggleMenu; }
    
        InputAction navigateAction = playerInput.actions.FindAction("Navigate");
        if (navigateAction != null) { navigateAction.Enable(); navigateAction.performed += OnNavigate; }
    
        InputAction submitAction = playerInput.actions.FindAction("Submit");
        if (submitAction != null) { submitAction.Enable(); submitAction.performed += OnSubmit; }
    }

    void UnsubscribeFromPlayer(PlayerInput playerInput)
    {
        InputAction pauseAction = playerInput.actions.FindAction("Pause");
        if (pauseAction != null) pauseAction.performed -= OnToggleMenu;
    
        InputAction navigateAction = playerInput.actions.FindAction("Navigate");
        if (navigateAction != null) navigateAction.performed -= OnNavigate;
    
        InputAction submitAction = playerInput.actions.FindAction("Submit");
        if (submitAction != null) submitAction.performed -= OnSubmit;
    }

    public void Resume()
    {
        foreach (PlayerController controller in FindObjectsByType<PlayerController>(FindObjectsSortMode.None))
        {
            controller.TryChangeInputActionMap("Player", out _);
        }
        Debug.Log("Resume game");
        m_pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        m_pauseMenuVolume.weight = 0;
    }

    public void Pause()
    {
        foreach (PlayerController controller in FindObjectsByType<PlayerController>(FindObjectsSortMode.None))
        {
            controller.TryChangeInputActionMap("UI", out _);
        }
        m_pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        m_pauseMenuVolume.weight = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_pauseMenuNavigation.OpenMenu();
       
    }

    public void RestartGame()
    {
        foreach (PlayerInput playerInput in PlayerInput.all)
        {
            UnsubscribeFromPlayer(playerInput);
        }
    }
    void OnNavigate(InputAction.CallbackContext context)
    {
        if (!m_pauseMenuUI.activeSelf) return;
        
        if (Time.unscaledTime - m_lastMovement < m_movementCoolDown) return;
        m_lastMovement = Time.unscaledTime;
        
        float y = context.ReadValue<Vector2>().y;
        if (y > 0.5) m_pauseMenuNavigation.MoveUp();
        else if (y < -0.5f) m_pauseMenuNavigation.MoveDown();
    }

    void OnSubmit(InputAction.CallbackContext context)
    {
        if (!m_pauseMenuUI.activeSelf) return;
        m_pauseMenuNavigation.Confirm();
    }

    void OnToggleMenu(InputAction.CallbackContext context)
    {
        Debug.Log($"OnToggleMenu: {m_pauseMenuUI.activeSelf}");
        if (!m_pauseMenuUI.activeSelf) Pause();
        else Resume();
    }
}
