using System;
using UnityEngine.InputSystem;

[Serializable]
public class MovementTutorialStep : TutorialStep
{
    IPlayerController[] m_playerControllers;
    int m_playersCompletedSteps;

    public void InjectPlayerControllers(IPlayerController[] playerControllers)
    {
        if (playerControllers.Length == 0) throw new ArgumentException("No player controllers found.");
        m_playerControllers = playerControllers;
        m_playersCompletedSteps = 0;
        foreach (IPlayerController controller in m_playerControllers)
        {
            if (!controller.TryGetCurrentInputActionMap(out InputActionMap map)) continue;
            map.FindAction("Move").performed += OnPlayerCompletedStep;
        }
    }

    protected override void StartStepInternal()
    {
        
    }

    protected override void EndStepInternal()
    {
        foreach (IPlayerController controller in m_playerControllers)
        {
            if (!controller.TryGetCurrentInputActionMap(out InputActionMap map)) continue;
            map.FindAction("Move").performed -= OnPlayerCompletedStep;
        }
    }

    void OnPlayerCompletedStep(InputAction.CallbackContext context)
    {
        m_playersCompletedSteps++;
        if (m_playersCompletedSteps < m_playerControllers.Length) return;
        EndStep();
    }
}