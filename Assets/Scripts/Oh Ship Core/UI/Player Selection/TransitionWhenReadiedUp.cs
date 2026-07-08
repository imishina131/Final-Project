using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;

public class TransitionWhenReadiedUp : MonoBehaviour, IDependencyProvider
{
    [Provide, UsedImplicitly] TransitionWhenReadiedUp GetReadyUp() => this;
    [SerializeField] int m_transitionThreshold = 2;
    [SerializeField] string m_sceneToTransitionTo = "Build Scene";
    [SerializeField] LoadingScreen loading;
    int m_readyUpCount;
    [Inject] ISceneTransitioner m_sceneTransitioner;
    public void OnPlayerReadyUp()
    { 
        m_readyUpCount++;
        if (m_readyUpCount >= m_transitionThreshold)
        {
            //m_sceneTransitioner.LoadScene(m_sceneToTransitionTo, 0.5f);
            loading.LoadScene(m_sceneToTransitionTo);
        }
    }

    public void OnPlayerNotReadyUp()
    {
        m_readyUpCount--;
    }
}