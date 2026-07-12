using UnityEngine;

public class PlayerLookRaycaster : MonoBehaviour
{
    [SerializeField] float m_promptRange = 2;
    [SerializeField] LayerMask m_layersToCheck;
    [SerializeField] InterfaceReference<IPromptDisplay> m_activeDisplay;
    IPromptDisplay m_highlighted;
    IPromptProvider m_currentProvider;

    private int m_nullProviderFrames = 0;
    private int m_nullProviderThreshold = 10;
    
    void LateUpdate()
    {
        IPromptProvider provider = null;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, m_promptRange, m_layersToCheck)) 
            hit.collider.TryGetComponent(out provider);

        if (provider != null)
        {
            m_nullProviderFrames = 0;
            if (provider == m_currentProvider) return;
            m_activeDisplay.Value.HidePrompt(m_currentProvider);
            m_currentProvider = provider;
            m_activeDisplay.Value.ShowPrompt(m_currentProvider);
        }
        else
        {
            m_nullProviderFrames++;
            if (m_nullProviderFrames >= m_nullProviderThreshold)
            {
                if (m_currentProvider == null) return;
                m_activeDisplay.Value.HidePrompt(m_currentProvider);
                m_currentProvider = null;
            }
        }
        /*IPromptProvider provider = null;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, m_promptRange, m_layersToCheck)) hit.collider.TryGetComponent(out provider);
        if (provider == m_currentProvider) return;
        m_activeDisplay.Value.HidePrompt(m_currentProvider);
        m_currentProvider = provider;
        m_activeDisplay.Value.ShowPrompt(m_currentProvider);*/
    }
}
