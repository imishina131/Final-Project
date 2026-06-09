using Codice.CM.Common;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class Stats
{
    readonly StatData baseStats;
    readonly StatBroker mediator;

    [SerializeField] StatData hungerStat;
    [SerializeField] StatData thirstStat;

    public StatBroker Mediator => mediator;

    private Dictionary<StatData, float> stats;

    private void Awake()
    {
        stats = new Dictionary<StatData, float>();
        SetDictionary();
    }

    private void SetDictionary()
    {
        stats.Add(hungerStat, 50);
        stats.Add(thirstStat, 25);
        Debug.Log($"Hunger: {stats[hungerStat]}, Thirst: {stats[thirstStat]}");

    }

    public Stats(StatBroker mediator, StatData baseStats)
    {
        this.mediator = mediator;
        this.baseStats = baseStats;
    }

    public float GetStat(StatData stat)
    {
        if (!stats.TryGetValue(stat, out float baseValue))
            return 0f;

        var query = new StatQuery(stat, baseValue);

        mediator.PerformStatQuery(this, query);

        return query.Value;
    }


    public void Update(float deltaTime)
    {
        mediator.UpdateStats(deltaTime);
    }



}
