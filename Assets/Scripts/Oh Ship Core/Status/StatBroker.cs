using OhShip.ShipCore;
using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages the application of stat modifiers to any stat. Perform a <see cref="StatQuery"/> to get the current value of a stat with <see cref="StatModifier"/>s applied.
/// </summary>
public class StatBroker
{
    readonly LinkedList<StatModifier> m_modifiers = new();
    EventHandler<StatQuery> m_queries;
    /// <summary>
    /// Performs a <see cref="StatQuery"/> to get the current value of a stat with <see cref="StatModifier"/>s applied.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that sent the <see cref="StatQuery"/></param>
    /// <param name="query">A <see cref="StatQuery"/> defined by the value we are querying</param>
    public void PerformStatQuery(object sender, StatQuery query) => m_queries?.Invoke(sender, query);
    /// <summary>
    /// Adds a <see cref="StatModifier"/> to the <see cref="StatBroker"/>. Will be applied to any <see cref="StatQuery"/> sent to the <see cref="PerformStatQuery"/> method.
    /// </summary>
    /// <param name="modifier">The <see cref="StatModifier"/> to add</param>
    public void AddModifier(StatModifier modifier)
    {
        modifier.OnDisposed += _ =>
        {
            m_modifiers.Remove(modifier);
            m_queries -= modifier.Handle;
        };
        m_modifiers.AddLast(modifier);
        m_queries += modifier.Handle;
    }
    /// <summary>
    /// Updates all <see cref="StatModifier"/>s in applied to the <see cref="StatBroker"/>
    /// </summary>
    /// <param name="deltaTime">The time since the last update</param>
    public void UpdateStats(float deltaTime)
    {
        LinkedListNode<StatModifier> node = m_modifiers.First;
        while (node != null)
        {
            node.Value.Update(deltaTime);
            node = node.Next;
        }
        node = m_modifiers.First;
        while(node != null)
        {
            LinkedListNode<StatModifier> next = node.Next;
            if (node.Value.MarkedForRemoval) node.Value.Dispose();
            node = next;
        }
    }
}