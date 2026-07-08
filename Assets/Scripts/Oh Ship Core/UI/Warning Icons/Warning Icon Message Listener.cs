using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.UI;

public class WarningIconMessageListener : MonoBehaviour
{
    [Header("These are icons that appear on specific players")] [SerializeField]
    WarningIcon[] m_warningIcons;

    [Header("These are the icons that appear based on low fuel, steam, etc")] [SerializeField]
    WarningIcon[] m_globalWarningIcons;

    [Header("This is the physical signs in the world")] [SerializeField]
    WorldWarningIcon[] m_worldWarningIcons;

    private INotificationMessenger m_messenger;
    private Dictionary<string, Coroutine> m_activeWarningPerCategory = new();
    private Dictionary<string, WarningIcon> m_activeWarningIconPerCategory = new();

    [Inject, UsedImplicitly]
    void InjectMessenger(INotificationMessenger messenger)
    {
        m_messenger = messenger;

        foreach (WarningIcon warningIcon in m_globalWarningIcons)
        {
            m_messenger.TrySubscribe(warningIcon.EnableMessage, () => StartCoroutine(warningIcon.EnableWarning()));
            m_messenger.TrySubscribe(warningIcon.EnableMessage, () => StartCoroutine(warningIcon.FlashWarningLabel(true)));
            m_messenger.TrySubscribe(warningIcon.DisableMessage, () => StartCoroutine(warningIcon.DisableWarning()));
            m_messenger.TrySubscribe(warningIcon.DisableMessage, () => StartCoroutine(warningIcon.FlashWarningLabel(false)));
        }
    }

    public void OnPlayerControllerConnect(IPlayerController playerController)
    {
        Debug.Log($"OnPlayerControllerConnect called, messenger null: {m_messenger == null}");
        if (!playerController.TryGetPlayerIndex(out int playerIndex)) return;
        Debug.Log($"Subscribing for player{playerIndex}");

        foreach (WarningIcon warningIcon in m_warningIcons)
        {
            WarningIcon captured = warningIcon;
            m_messenger.TrySubscribe($"{captured.EnableMessage} player{playerIndex}",
                () => ShowWarning(captured));
            m_messenger.TrySubscribe($"{captured.DisableMessage} player{playerIndex}",
                () => StartCoroutine(captured.DisableWarning()));
            m_messenger.TrySubscribe($"{captured.DisableMessage} player{playerIndex}",
                () => StartCoroutine(captured.FlashWarningLabel(false)));
        }

        foreach (WorldWarningIcon worldIcon in m_worldWarningIcons)
        {
            m_messenger.TrySubscribe($"{worldIcon.EnableMessage}", () => worldIcon.Enable());
            m_messenger.TrySubscribe($"{worldIcon.DisableMessage}", () => worldIcon.Disable());
        }

        void ShowWarning(WarningIcon warningIcon)
        {
            string category = warningIcon.Category;

            if (m_activeWarningIconPerCategory.TryGetValue(category, out WarningIcon previous))
            {
                if (m_activeWarningPerCategory.TryGetValue(category, out Coroutine activeCoroutine))
                    StopCoroutine(activeCoroutine);
                StartCoroutine(previous.DisableWarning());
                m_activeWarningPerCategory.Remove(category);
                m_activeWarningIconPerCategory.Remove(category);
            }

            if (string.IsNullOrEmpty(category))
            {

                StartCoroutine(warningIcon.EnableWarning());
                StartCoroutine(warningIcon.FlashWarningLabel(true));
                return;
            }

            m_activeWarningIconPerCategory[category] = warningIcon;
            m_activeWarningPerCategory[category] = StartCoroutine(warningIcon.EnableWarning());
            StartCoroutine(warningIcon.FlashWarningLabel(true));
        }

    }


    [Serializable]
    struct WarningIcon
    {
        public CanvasGroup CanvasGroup;
        public string EnableMessage;
        public string DisableMessage;
        public Sprite[] sprites;
        public Image image;
        public string Category;

        public IEnumerator EnableWarning()
        {
            yield return CanvasGroup.FadeToOpacity(1, 0.5f);

        }

        public IEnumerator DisableWarning()
        {
            yield return CanvasGroup.FadeToOpacity(0, 0.5f);
        }

        public IEnumerator FlashWarningLabel(bool enable)
        {
            if (!image || sprites.Length == 0) yield break;
            if (!enable || sprites.Length < 2)
            {
                image.sprite = sprites[0];
                yield break;
            }

            float flashDuration = 2f;
            float flashInterval = 0.5f;
            float elapsed = 0f;
            bool toggle = false;
            while (elapsed < flashDuration)
            {
                image.sprite = sprites[toggle ? 1 : 0];
                toggle = !toggle;
                yield return new WaitForSeconds(flashInterval);
                elapsed += flashInterval;
            }

            Debug.Log("Display");
            image.sprite = sprites[0];
        }
    }

    [Serializable]
    struct WorldWarningIcon
    {
        public string GameObjectTag;
        public string EnableMessage;
        public string DisableMessage;

        public void Enable()
        {
            GameObject obj = GameObject.FindWithTag(GameObjectTag);

            if (obj != null)
            {
                obj.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        public void Disable()
        {
            GameObject obj = GameObject.FindWithTag(GameObjectTag);
            if (obj != null)
            {
                obj.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}

