using UnityEngine;
using System.Collections.Generic;

public class Stats: MonoBehaviour
{

    [SerializeField] StatData hungerStat;
    [SerializeField] StatData thirstStat;

    public StatBroker broker = new StatBroker();


    private Dictionary<StatData, float> stats;


    private void Update()
    {
        StatQuery value = new StatQuery(hungerStat, 1f);
        broker.PerformStatQuery(this, value);
        //Debug.Log(value.Value);
    }




}
