using System;
/// <summary>
/// Represents a tank state. This can be used to represent different tank behaviors like filling, draining, or neutral
/// </summary>
public interface IFillState
{
    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    /// <param name="currentFill">The current fill amount that our tank is at</param>
    void OnEventStarted(float currentFill);
    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    void OnEventEnded();
    /// <summary>
    /// Called when the player tries to increase the fill amount.
    /// </summary>
    void HandleIncrease();
    /// <summary>
    /// Called when the player tries to decrease the fill amount.
    /// </summary>
    void HandleDecrease();
    /// <summary>
    /// Called when the state changes the desired fill amount
    /// </summary>
    public event Action<float> OnFillChange; 
}