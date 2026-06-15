using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PromptDisplay : MonoBehaviour, IPromptDisplay
{
    [SerializeField] Camera m_connectedCamera;
    [SerializeField] RectTransform m_promptContainer;
    [SerializeField] float m_referenceDistance = 2f;
    [SerializeField] SerializableDictionary<string, GameObject> m_promptPrefabs = new();
    readonly Dictionary<IPromptProvider, PromptInfo> m_activePrompts = new();
    readonly Dictionary<string, ObjectPool<PromptInfo>> m_pools = new();

    void Start()
    {
        foreach ((string key, GameObject value) in m_promptPrefabs)
        {
            ObjectPool<PromptInfo> info = new(
                createFunc: () =>
                {
                    GameObject instance = Instantiate(value, m_promptContainer);
                    return new()
                    {
                        Prompt = instance.GetComponent<RectTransform>(),
                        CanvasGroup = instance.GetComponent<CanvasGroup>() ?? instance.AddComponent<CanvasGroup>(),
                        Widget = key
                    };
                },
                actionOnGet:     info => { info.CanvasGroup.alpha = 1f; info.CanvasGroup.blocksRaycasts = true; },
                actionOnRelease: info => { info.CanvasGroup.alpha = 0f; info.CanvasGroup.blocksRaycasts = false; },
                actionOnDestroy: info => Destroy(info.Prompt.gameObject)
            );
            PromptInfo item = info.Get();
            info.Release(item);
            m_pools.Add(key, info);
        }
        
    }

    void Update()
    {
        if (m_activePrompts.Count <= 0) return;
        foreach (KeyValuePair<IPromptProvider, PromptInfo> prompt in m_activePrompts)
        {
            Vector3 worldPos = prompt.Key.GetWidgetWorldPosition();
            Vector3 screenPos = m_connectedCamera.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_promptContainer, screenPos, m_connectedCamera, out Vector2 localPoint);
            prompt.Value.Prompt.anchoredPosition = localPoint;
         //   float distance = Vector3.Distance(m_connectedCamera.transform.position, worldPos);
          //  prompt.Value.Prompt.localScale = Vector3.one * (m_referenceDistance / distance);
        }
    }

    public void ShowPrompt(IPromptProvider prompt)
    {

        if (prompt is null) return;
        if (m_activePrompts.ContainsKey(prompt)) return;
        if (!m_pools.TryGetValue(prompt.GetPromptData().AssociatedWidget, out ObjectPool<PromptInfo> pool)) return;
        PromptInfo info = pool.Get();
        prompt.GetPromptData().Apply(info.Prompt.gameObject);
        Vector3 worldPos = prompt.GetWidgetWorldPosition();
        Vector3 screenPos = m_connectedCamera.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_promptContainer, screenPos, m_connectedCamera, out Vector2 localPoint);
        info.Prompt.anchoredPosition = localPoint;
        
        if (prompt is IProgressRepairProvider progressProviderRef &&
            info.Prompt.GetComponent<ShipHoleVisual>() is ShipHoleVisual visual)
        {
            progressProviderRef.AddProgressSubscriber(visual.UpdateRadialProgress);
        }
        m_activePrompts.Add(prompt, info);
    }

    public void HidePrompt(IPromptProvider prompt)
    {
        if (prompt is null) return;
        if (!m_activePrompts.Remove(prompt, out PromptInfo info)) return;
        
        if (prompt is IProgressRepairProvider progressProviderRef && info.Prompt.GetComponent<ShipHoleVisual>() is ShipHoleVisual visual)
        {
            progressProviderRef.RemoveProgressSubscriber(visual.UpdateRadialProgress);
        }
        if (m_pools.TryGetValue(info.Widget, out ObjectPool<PromptInfo> pool)) pool.Release(info);
    }

    class PromptInfo
    {
        public RectTransform Prompt;
        public string Widget;
        public CanvasGroup CanvasGroup;
    }
}