using System;
using MatrixUtils.DependencyInjection;
using TextMateSharp.Grammars;
using UnityEngine;

public class FuelWarning : MonoBehaviour
{
    private bool m_activeAlert = false;
    [Inject] INotificationMessenger m_notificationMessenger;

    private void Start()
    {
        FindAnyObjectByType<Injector>().Inject(this);
    }

    public void NotifyLowFuelSystem(bool fuelIsLow)
    {
        if (fuelIsLow)
        {
            if (m_activeAlert) return;
            m_notificationMessenger.TryNotify("enable coal");
            m_activeAlert = true;
        }
        else
        {
            if (!m_activeAlert) return;
            m_notificationMessenger.TryNotify("disable coal");
            m_activeAlert = false;
        }
        
    }
}
