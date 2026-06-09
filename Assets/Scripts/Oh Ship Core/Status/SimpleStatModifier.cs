using System;
/// <summary>
/// A simple stat modifier that applies a function to the value of a stat via a passed in <see cref="Func{T, TResult}"/> and accompanying <see cref="StatData"/>
/// </summary>
/// 
namespace OhShip.ShipCore
{

    public class SimpleStatModifier : StatModifier
    {
        readonly StatData m_stat;
        readonly Func<float, float> m_modifier;
        public SimpleStatModifier(Func<float, float> modifier, StatData stat)
        {
            m_modifier = modifier;
            m_stat = stat;
        }
        /// <inheritdoc/>
        public override void Update(float deltaTime)
        {

        }
        /// <inheritdoc/>
        public override void Handle(object sender, StatQuery query)
        {
            if (query.Data != m_stat) return;
            query.Value = m_modifier(query.Value);
        }
    }
}