/// <summary>
/// Represents an object that can be interacted with by an <see cref="IInteractor"/>
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Begins an interaction with the <see cref="IInteractor"/>
    /// </summary>
    /// <param name="interactor">The <see cref="IInteractor"/> to begin the interaction with</param>
    /// <returns>An <see cref="InteractionSession"/> representing the current state of the interaction</returns>
    InteractionSession BeginInteraction(IInteractor interactor);
}
