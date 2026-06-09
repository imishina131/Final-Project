using MatrixUtils.Attributes;
using UnityEngine;
/// <summary>
/// Interacting with this object will move the <see cref="IInteractor"/> to the position specified in the inspector
/// </summary>
public class HatchInteractable : MonoBehaviour, IInteractable, IPromptProvider
{
    [SerializeReference, ClassSelector] PromptData m_promptData;
    //[SerializeField] Transform m_widgetPosition;
    [SerializeField] Transform m_outPosition;
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        ITransitionHider transitionHider = interactor.GetAssociatedGameObject().transform.root.GetComponent<IPlayerControllable>().GetActivePlayerController().GetAssociatedGameObject().transform.root.GetComponentInChildren<ITransitionHider>();
        Rigidbody rb = interactor.GetAssociatedGameObject().transform.root.GetComponent<Rigidbody>();
        transitionHider.FadeIn(0.2f, () =>
        {
            rb.MovePosition(m_outPosition.position);
            transitionHider.FadeOut(0.2f);
        });
        return null;
    }

    public PromptData GetPromptData() => m_promptData;
    public Vector3 GetRequestedWorldPosition() => transform.position;
}
