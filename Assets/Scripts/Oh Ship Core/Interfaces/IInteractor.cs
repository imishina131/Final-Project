/// <summary>
/// Represents an object that can interact with other <see cref="IInteractable"/>s.
/// </summary>
public interface IInteractor
{
    /// <summary>
    /// Returns true if the <see cref="IInteractor"/> is interacting with a <see cref="IInteractable"/>
    /// </summary>
    /// <returns>Whether the <see cref="IInteractor"/> is interacting with a <see cref="IInteractable"/></returns>
    bool IsInteracting();
    /// <summary>
    /// Returns the current <see cref="InteractionSession"/> if the <see cref="IInteractor"/> is interacting with a <see cref="IInteractable"/>
    /// </summary>
    /// <returns>The current <see cref="InteractionSession"/></returns>
    InteractionSession GetSession();
    /// <summary>
    /// Requests the <see cref="InteractionSession"/> to be transferred to this <see cref="PlayerInteractor"/>
    /// </summary>
    /// <param name="session">The <see cref="InteractionSession"/> to request the transfer of</param>
    bool RequestSessionTransfer(InteractionSession session);
}