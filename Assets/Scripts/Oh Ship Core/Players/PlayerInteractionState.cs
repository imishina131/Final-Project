using System;
using System.Collections.Generic;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionState : MonoBehaviour
{
    [SerializeField] private HashSet<InteractionTag> m_interactionTags =  new HashSet<InteractionTag>();
    [Inject] INotificationMessenger m_notificationMessenger;
    private int m_playerIndex = -1;
    
    public int PlayerIndex => m_playerIndex;
    private void Start()
    { 
        FindAnyObjectByType<Injector>().Inject(this);
       
    }

    public void OnPlayerControllerConnected(IPlayerController playerController)
    {
        if (playerController.TryGetPlayerIndex(out int playerIndex))
        {
            Debug.Log($"Player {playerIndex} connected");
            m_playerIndex = playerIndex;
        }
    }

    public void OnPlayerControllerDisconnected(IPlayerController playerController)
    {
            
    }

    public void AddInteractionTag(InteractionTag interactionTag)
    {
        if(!m_interactionTags.Add(interactionTag)) return;
        Debug.Log($"Firing notification: 'added {interactionTag}'");
        m_notificationMessenger.TryNotify($"added {interactionTag}"); // world objects
        m_notificationMessenger.TryNotify($"added {interactionTag} player{m_playerIndex}"); // HUD
    }

    public void RemoveInteractionTag(InteractionTag interactionTag)
    {
        if(!m_interactionTags.Contains(interactionTag)) return;
        m_interactionTags.Remove(interactionTag);
        m_notificationMessenger.TryNotify($"removed {interactionTag}"); // world objects
        m_notificationMessenger.TryNotify($"removed {interactionTag} player{m_playerIndex}"); // HUD
    }
    
    public bool CheckInteractionTag(InteractionTag interactionTag)
    {
        return  m_interactionTags.Contains(interactionTag);
    }

    private void Update()
    {
        if (m_interactionTags.Count > 0)
            Debug.Log($"Active tags on: {gameObject.name} - {string.Join(", ", m_interactionTags)}");
    }
}
