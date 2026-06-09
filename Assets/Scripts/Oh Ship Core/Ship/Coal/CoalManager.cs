using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class CoalManager : MonoBehaviour, IInteractable, IPlayerControllable, IPromptProvider
{
    [SerializeField] private string _widgetForPrompt = "interact";
    [SerializeField] private SO_CoalData _coalData;
    [SerializeField] private int _howManyInputs = 5;
    [SerializeField] private string m_coalMiniGameActionMap = "Shovel Coal";
    [Range(0.1f,1)]
    [SerializeField] private float m_amountOfPressureToSendReference = .2f;

    [SerializeField] SteamPressureSystem m_steamPressureSystem;
    [SerializeField] private float m_timeLimitReference = 10f;
    
    
    private IPlayerController m_activePlayerController;
    private IPlayerControllable m_activePlayerControl;
    private InteractionSession m_currentInteractionSession;
    public string[] m_inputsForQTE;
    private int m_index;
    private int m_correctInputCounter;
    private CoalUI m_coalUI;
    private float m_timeLimit;
    private float m_pressureToSend;
    public InteractionSession BeginInteraction(IInteractor interactor)
    { 
        m_activePlayerControl = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
        m_activePlayerController = m_activePlayerControl.GetActivePlayerController();
        m_activePlayerController.ChangeControlledEntity(this);

        m_currentInteractionSession = new InteractionSession(interactor, this);
        m_currentInteractionSession.OnEnded += () => m_activePlayerController.ChangeControlledEntity(m_activePlayerControl);
        m_inputsForQTE = new string[_howManyInputs];
        m_index = 0;
        m_correctInputCounter = 0;
        m_timeLimit = m_timeLimitReference;
        m_pressureToSend = m_amountOfPressureToSendReference;
        for (var i = 0; i < _howManyInputs; i++)
        {
            int randomInput = Random.Range(0,_coalData.PossibleInputs.Length);
            m_inputsForQTE[i] = _coalData.PossibleInputs[randomInput].InputName;
        }
       
        SetUpUIConnection();
       
        return m_currentInteractionSession;
    }

    public void OnControlRequested(IPlayerController player)
    {
        m_activePlayerController = player;
        
        if (!player.ChangeInputActionMap(m_coalMiniGameActionMap, out InputActionMap map))
        {
            Debug.LogError("Failed to assign input actions to player, reverting control to default.");
            player.ChangeControlledEntity(null);
            return;
        }

        InputAction leftButton = map.FindAction("Left");
        leftButton.performed += QTEButtonPressed;
        InputAction rightButton = map.FindAction("Right");
        rightButton.performed += QTEButtonPressed;
        InputAction downButton = map.FindAction("Down");
        downButton.performed += QTEButtonPressed;
        InputAction interact = map.FindAction("Interact");
        interact.performed += HandleInteract;
    }

    void QTEButtonPressed(InputAction.CallbackContext context)
    {
        if (context.action.name == m_inputsForQTE[m_index])
        {
            Debug.Log("Correct");
            m_coalUI.CorrectButtonPressed(m_index);
            m_correctInputCounter++;
        }
        else
        {
            Debug.Log("Incorrect");
            m_coalUI.IncorrectButtonPressed(m_index);
        }
        m_index++;
        
        if(m_index >= m_inputsForQTE.Length)
        {
            Debug.Log("Inputs completed");
            m_pressureToSend *=  ((float)m_correctInputCounter / m_inputsForQTE.Length);
            m_steamPressureSystem.IncreaseSteamPressure(m_pressureToSend);
            Debug.Log("Sent " + m_pressureToSend + " of pressure");
            m_currentInteractionSession.End();
        }
    }

    private void Update()
    {
        if (m_activePlayerController == null) return;
        
        m_timeLimit -= Time.deltaTime;
        if (m_timeLimit <= 0f)
        {
            m_currentInteractionSession.End();
        }
        
    }

    public void OnControlReleased()
    {
        if (m_activePlayerController == null) throw new("Player controller is null, cannot release control.");
        if (!m_activePlayerController.GetCurrentInputActionMap(out InputActionMap map)) throw new("Player controller is not null, but input action map is null...");
        
        InputAction leftButton = map.FindAction("Left");
        leftButton.performed -= QTEButtonPressed;
        InputAction rightButton = map.FindAction("Right");
        rightButton.performed -= QTEButtonPressed;
        InputAction downButton = map.FindAction("Down");
        downButton.performed -= QTEButtonPressed;
        InputAction interact = map.FindAction("Interact");
        interact.performed -= HandleInteract;
        m_coalUI.HideUI();
        m_activePlayerController = null;
    }

    public IPlayerController GetActivePlayerController() => m_activePlayerController;
    

    public GameObject GetAssociatedGameObject() => gameObject;
    
    void HandleInteract(InputAction.CallbackContext context) => m_currentInteractionSession.End();
    
    private void SetUpUIConnection()
    {
        GameObject player = m_activePlayerController.GetAssociatedGameObject();
        
        m_coalUI = player.GetComponentInChildren<CoalUI>();
        
        m_coalUI.DisplayPasswordVisuals(m_inputsForQTE);
    }

    public PromptData GetPromptData()
    {
        return new PromptData {AssociatedWidget = _widgetForPrompt };
    }

    public Vector3 GetRequestedWorldPosition()
    {
        return transform.position;
    }
}


