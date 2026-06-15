using System;
using UnityEngine;

public interface IProgressRepairProvider
{
    void AddProgressSubscriber(Action<float> sub);
    void RemoveProgressSubscriber(Action<float> sub);
}
