using System;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// Represents an object that can display a prompt to the user. Used for things like notifying the player that an object is interactable.
/// </summary>
public interface IPromptProvider
{
    [Pure] public PromptData GetPromptData();
    [Pure] public string GetRequestedWidgetName();
    [Pure] public Vector3 GetRequestedWorldPosition();
}
