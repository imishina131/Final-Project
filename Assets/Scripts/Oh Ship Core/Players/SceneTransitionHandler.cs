using System.Collections;
using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionHandler : PersistentService<ISceneTransitioner>, ISceneTransitioner
{
    bool m_isTransitioning;
    [SerializeField] CanvasGroup m_fadeCanvasGroup;
    [Provide, UsedImplicitly] ISceneTransitioner GetTransitioner() => this;
    public bool TransitionToScene(string sceneName, float duration)
    {
        if (m_isTransitioning) return false;
        StartCoroutine(TransitionToSceneAsync(sceneName, duration));
        return true;
    }

    IEnumerator TransitionToSceneAsync(string sceneName, float duration)
    {
        m_isTransitioning = true;
        m_fadeCanvasGroup.blocksRaycasts = true;
        yield return m_fadeCanvasGroup.FadeToOpacity(1, duration);

        if (sceneName == "GameOver" || sceneName == "CinematicScene")
        {
            foreach (PlayerController controller in FindObjectsByType<PlayerController>(FindObjectsSortMode.None))
            {
                Destroy(controller.gameObject);
            }
        }
        
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return m_fadeCanvasGroup.FadeToOpacity(0, duration);
        m_fadeCanvasGroup.blocksRaycasts = false;
        m_isTransitioning = false;
    }
}