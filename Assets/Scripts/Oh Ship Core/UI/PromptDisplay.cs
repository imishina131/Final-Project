using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PromptDisplay : MonoBehaviour
{
    [SerializeField] Camera m_connectedCamera;
    [SerializeField] RectTransform m_promptContainer;
    [SerializeField] SerializableDictionary<string, GameObject> m_promptPrefabs = new();
    readonly Dictionary<IPromptProvider, RectTransform> m_activePrompts = new();
    readonly Dictionary<string, ObjectPool<RectTransform>> m_pools = new();

    void Start()
    {
        foreach (KeyValuePair<string, GameObject> promptPrefab in m_promptPrefabs)
        {
            m_pools.Add(promptPrefab.Key, new(
                () => Instantiate(promptPrefab.Value).GetComponent<RectTransform>(),
                rectTransform => rectTransform.gameObject.SetActive(true), 
                rectTransform => rectTransform.gameObject.SetActive(false)
            ));
        }
    }
    public void AddPrompt(IPromptProvider promptProvider)
    {
        if (m_activePrompts.ContainsKey(promptProvider)) return;
        if(!m_pools.TryGetValue(promptProvider.GetRequestedWidgetName(), out ObjectPool<RectTransform> pool)) return;
        RectTransform promptRect = pool.Get();
        promptProvider.GetPromptData().Apply(promptRect.gameObject);
        m_activePrompts.Add(promptProvider, promptRect);
    }

    public void RemovePrompt(IPromptProvider promptProvider)
    {
        if (!m_activePrompts.Remove(promptProvider, out RectTransform promptRect)) return;
        if (m_pools.TryGetValue(promptProvider.GetRequestedWidgetName(), out ObjectPool<RectTransform> pool)) pool.Release(promptRect);
    }

    void Update()
    {
        if (m_activePrompts.Count <= 0) return;
        foreach (KeyValuePair<IPromptProvider, RectTransform> prompt in m_activePrompts)
        {
            Vector3 screenPos = m_connectedCamera.WorldToScreenPoint(prompt.Key.GetRequestedWorldPosition());
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_promptContainer, screenPos, m_connectedCamera, out Vector2 localPoint);
            prompt.Value.anchoredPosition = localPoint;
        }
    }

}