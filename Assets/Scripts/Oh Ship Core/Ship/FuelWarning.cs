using System;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.Events;

public class FuelWarning : MonoBehaviour
{
    private bool m_activeAlert = false;
    [Inject] INotificationMessenger m_notificationMessenger;
    
    [SerializeField] private UnityEvent OnFuelLevelUpdate = new  UnityEvent();
    private float m_fuelLevel;
    [SerializeField] private UnityEvent OnWaterLevelUpdate = new  UnityEvent();
    [SerializeField] private ProcedurallyAnimatedElement m_fuelGauge;
    [SerializeField] private ProcedurallyAnimatedElement m_waterGauge;

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
    
    

    private void Update()
    {
        //m_fuelLevel = OnFuelLevelUpdate.Invoke();
       //m_fuelGauge.Transform.localRotation = Quaternion.Euler(0,0,m_fuelGauge.GetNextAngle(, m_fuelGauge.Transform.localRotation.eulerAngles.z));
        
    }

    [Serializable]
    class ProcedurallyAnimatedElement
    {
        public Transform Transform;
        public float MinAngle;
        public float MaxAngle;
        float m_velocity;

        public float GetNextAngle(float normalizedDesiredAngle, float currentAngle)
        {
            float desiredWheelAngle = Mathf.Lerp(MinAngle, MaxAngle, normalizedDesiredAngle);
            return Mathf.SmoothDampAngle(currentAngle, desiredWheelAngle, ref m_velocity, 0.1f);
        }
    }
}
