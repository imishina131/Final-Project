using UnityEngine;

public class SimpleInteractable : MonoBehaviour, IInteractable
{
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        Debug.Log("Simple Interactable Interacted");
        return new(interactor, this);
    }

    public void EndInteraction(InteractionSession session)
    {
        Debug.Log("Simple Interactable Ended Interaction");
    }
}
