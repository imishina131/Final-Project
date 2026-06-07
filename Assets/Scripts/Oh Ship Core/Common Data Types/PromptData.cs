using UnityEngine;
/// <summary>
/// Data that can be applied to a UI widget via its <see cref="GameObject"/>
/// </summary>
public abstract class PromptData
{
    /// <summary>
    /// Applies the prompt data to a given UI widget
    /// </summary>
    /// <param name="gameObject">The <see cref="GameObject"/> to apply this prompt data to</param>
    public abstract void Apply(GameObject gameObject);
}
