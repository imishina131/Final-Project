using System.Collections;
using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.InputSystem;
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
        bool keepPlayerControllers = sceneName == "Build Scene";
        m_isTransitioning = true;
        m_fadeCanvasGroup.blocksRaycasts = true;
        yield return m_fadeCanvasGroup.FadeToOpacity(1, duration);

        if (!keepPlayerControllers)
        {
            PauseMenu pauseMenu = FindFirstObjectByType<PauseMenu>();
            foreach (PlayerController controller in FindObjectsByType<PlayerController>(FindObjectsSortMode.None))
            {
                PlayerInput playerInput = controller.GetComponent<PlayerInput>();
                if (playerInput != null && pauseMenu != null)
                {
                    pauseMenu.UnsubscribeFromPlayer(playerInput);
                }
                Destroy(controller.gameObject);
            }
        }
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return m_fadeCanvasGroup.FadeToOpacity(0, duration);
        m_fadeCanvasGroup.blocksRaycasts = false;
        m_isTransitioning = false;
    }
}