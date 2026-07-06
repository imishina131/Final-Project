using UnityEngine;

public class PlayerLookRaycaster : MonoBehaviour
{
    [SerializeField] float m_promptRange = 2;
    [SerializeField] LayerMask m_layersToCheck;
    [SerializeField] InterfaceReference<IPromptDisplay> m_activeDisplay;
    IPromptDisplay m_highlighted;
    IPromptProvider m_currentProvider;
    
    void Update()
    {
        IPromptProvider provider = null;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, m_promptRange, m_layersToCheck)) hit.collider.TryGetComponent(out provider);
        if (provider == m_currentProvider) return;
        m_activeDisplay.Value.HidePrompt(m_currentProvider);
        m_currentProvider = provider;
        m_activeDisplay.Value.ShowPrompt(m_currentProvider);
    }
}
