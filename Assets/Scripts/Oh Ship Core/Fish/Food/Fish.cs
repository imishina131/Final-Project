using UnityEngine;

public class Fish : FoodClass
{
    private Material m_material;
    private float m_cookedAmount;
    private CookState m_currentCookState = CookState.Raw;
    
    public override CookState CookStateRef {get{return m_currentCookState;}}
    
    public override CookingProcess CookingProcess => CookingProcess.OnGrill;

    private void Start()
    {
        m_material = GetComponent<MeshRenderer>().material;
        Reset();
    }

    public override void UpdateCookedAmount(float incomingAmount)
    {
        m_cookedAmount = incomingAmount;
        m_material.SetFloat("_Cooked_Amount", m_cookedAmount);
        
        CookState newState = DetermineCookState(m_cookedAmount);

        if (newState == m_currentCookState) return;
        m_currentCookState = newState;
    }

    public override void Reset()
    {
        if (m_currentCookState == CookState.Raw)
        {
            m_material.SetFloat("_Cooked_Amount", 0);
            m_cookedAmount = 0;
            m_currentCookState = CookState.Raw;
        }
    }
    
    public override float Eat()
    {
        return foodData.HungerRestored(m_currentCookState);
    }


    CookState DetermineCookState(float cookedAmount)
    {
        if (foodData.GetThreshold(CookState.Burnt) >= 0 && cookedAmount >= foodData.GetThreshold(CookState.Burnt))
            return CookState.Burnt;
        
        if (foodData.GetThreshold(CookState.Cooked) >= 0 && cookedAmount >= foodData.GetThreshold(CookState.Cooked))
            return CookState.Cooked;
        
        return CookState.Raw;
    }
}
