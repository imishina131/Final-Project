using System;
using UnityEngine.InputSystem;

[Serializable]
public class MovementTutorialStep : TutorialStep
{
    int m_playersCompletedSteps;
    TutorialContext m_context;

    protected override void StartStepInternal(TutorialContext context)
    {
        m_context = context;
        foreach (InterfaceReference<IPlayerController> movement in context.PlayerControllers)
        {
            if(!movement.Value.TryGetCurrentInputActionMap(out InputActionMap map)) return;
            map.FindAction("Move").performed += OnPlayerCompletedStep;
        }
    }
    protected override void EndStepInternal()
    {
        foreach (InterfaceReference<IPlayerController> controller in m_context.PlayerControllers)
        {
            if (!controller.Value.TryGetCurrentInputActionMap(out InputActionMap map)) continue;
            map.FindAction("Move").performed -= OnPlayerCompletedStep;
        }
    }

    void OnPlayerCompletedStep(InputAction.CallbackContext context)
    {
        m_playersCompletedSteps++;
        if (m_playersCompletedSteps < m_context.PlayerControllers.Length) return;
        EndStep();
    }
}