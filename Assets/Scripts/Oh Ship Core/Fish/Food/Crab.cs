using System;
using UnityEngine;

public class Crab : FoodClass
{
    private Material m_material;
    private float m_cookedAmount;
    private CookState m_currentCookState = CookState.Raw;
    [SerializeField] private ParticleSystem cookedVFX;
    private PlayerInteractionState playerInteractionState;
   
    [Header("Audio")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip cookingDone;

    public override CookState CookStateRef { get { return m_currentCookState; } }

    public override CookingProcess CookingProcess => CookingProcess.InPot;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        m_material = GetComponent<MeshRenderer>().material;
       
        playerInteractionState = GetComponentInParent<PlayerInteractionState>();
       
        Debug.Log(playerInteractionState);
    }


    public override void UpdateCookedAmount(float incomingAmount)
    {
        m_cookedAmount = incomingAmount;
        m_material.SetFloat("_Cooked_Amount", m_cookedAmount);

        CookState newState = DetermineCookState(m_cookedAmount);

        if (newState == m_currentCookState) return;
        m_currentCookState = newState;

        if(m_currentCookState == CookState.Cooked)
        {
            Debug.Log("Cooked");
            cookedVFX.Play();
            audioSource.PlayOneShot(cookingDone);
        }
    }

    public override void Reset()
    {
        m_material = GetComponent<MeshRenderer>().material;
        if (m_currentCookState == CookState.Raw)
        {
            m_material.SetFloat("_Cooked_Amount", 0);
            m_cookedAmount = 0;
            m_currentCookState = CookState.Raw;
        }
    }

    public override float Eat()
    {
        playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingFish);
        playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingCookedFish);
        playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingBurntFish);
 
        return m_foodData.HungerRestored(m_currentCookState);
    }


    CookState DetermineCookState(float cookedAmount)
    {
        if (m_foodData.GetThreshold(CookState.Burnt) >= 0 && cookedAmount >= m_foodData.GetThreshold(CookState.Burnt))
            return CookState.Burnt;

        if (m_foodData.GetThreshold(CookState.Cooked) >= 0 && cookedAmount >= m_foodData.GetThreshold(CookState.Cooked))
            return CookState.Cooked;

        return CookState.Raw;
    }

    
}
