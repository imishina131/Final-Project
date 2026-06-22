using System.Collections.Generic;
using MatrixUtils.Attributes;
using UnityEngine;
//This totally isn't going to turn into a state machine, not at all
public class TutorialOrchestrator : MonoBehaviour
{
    [SerializeReference, ClassSelector] List<TutorialStep> m_tutorialSteps = new();
    int m_activeStep;
    TutorialContext m_context;
    void Start()
    {
        AdvanceToNextStep();
    }
    
    void AdvanceToNextStep()
    {
        if (m_activeStep >= 0) m_tutorialSteps[m_activeStep].TryRemoveCompleteEvent(AdvanceToNextStep);
        int next = m_activeStep + 1;
        if (next >= m_tutorialSteps.Count) return;
        m_activeStep = next;
        TutorialStep step = m_tutorialSteps[m_activeStep];
        step.TryAddCompleteEvent(AdvanceToNextStep);
        step.StartStep(m_context);
    }
}