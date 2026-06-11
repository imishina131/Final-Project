using MatrixUtils.DependencyInjection;
using UnityEngine;

public class SceneTransitionerProxy : MonoBehaviour
{
    [SerializeField] float m_transitionDuration = 0.5f;
    [Inject] ISceneTransitioner m_sceneTransitioner;
    public void TransitionToScene(string sceneName) => m_sceneTransitioner.TransitionToScene(sceneName, m_transitionDuration);
    
}
