using UnityEngine;
using UnityEngine.InputSystem;

public class StoveInteractable : MonoBehaviour, IInteractable, IPromptProvider
{
    InteractionSession m_currentInteractionSession;
    [SerializeField] private GameObject fishToCook;
    private readonly string _widgetForPrompt = "interact";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        
        if (interactor.IsInteracting() || m_currentInteractionSession is { IsActive: true }) return null;
        
        fishToCook.SetActive(true);
        Debug.Log(fishToCook.name);
        
        return m_currentInteractionSession;
    }

    public PromptData GetPromptData()
    {
        return new PromptData { AssociatedWidget = _widgetForPrompt };
    }

    public Vector3 GetWidgetWorldPosition()
    {
       return transform.position;
    }
}
