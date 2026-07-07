using UnityEngine;
using UnityEngine.Audio;

public class Fish : FoodClass
{
    static readonly int s_cookedAmount = Shader.PropertyToID("_Cooked_Amount");
    private Material m_material;
    private float m_cookedAmount;
    private CookState m_currentCookState = CookState.Raw;
    [SerializeField] private ParticleSystem cookedVFX; 
    private HeldObjectHandler m_heldObjectHandler;
    Transform locationOfHeldObject;

    [Header("Audio")]
     private AudioSource audioSource;
    [SerializeField] private AudioClip cookingDone;

    public override CookState CookStateRef => m_currentCookState;

    public override CookingProcess CookingProcess => CookingProcess.OnGrill;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        m_material = GetComponent<MeshRenderer>().material;
        m_heldObjectHandler = GetComponentInParent<HeldObjectHandler>();
        Debug.Log(m_heldObjectHandler);
        locationOfHeldObject = m_heldObjectHandler.transform;
        Debug.Log(locationOfHeldObject);
        Debug.Log(playerInteractionState);
        
        //Reset();
    }

    public override void UpdateCookedAmount(float incomingAmount)
    {
        m_cookedAmount = incomingAmount;
        m_material.SetFloat(s_cookedAmount, m_cookedAmount);
        
        CookState newState = DetermineCookState(m_cookedAmount);

        if (newState == m_currentCookState) return;
        m_currentCookState = newState;

        if (m_currentCookState == CookState.Cooked)
        {
            cookedVFX.Play();
            audioSource.PlayOneShot(cookingDone);
        }
    }

    public override void Reset()
    {
        m_material = GetComponent<MeshRenderer>().material;
        if (m_currentCookState == CookState.Raw)
        {
            m_material.SetFloat(s_cookedAmount, 0);
            m_cookedAmount = 0;
            m_currentCookState = CookState.Raw;
        }
    }
    
    public override float Eat()
    {
        
        Debug.Log($"Eating - playerInteractionState player: {playerInteractionState?.gameObject.name}");
        Debug.Log($"HoldingCookedFish before remove: {playerInteractionState?.CheckInteractionTag(InteractionTag.HoldingCookedFish)}");
        playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingFish);
        playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingCookedFish);
        playerInteractionState.RemoveInteractionTag(InteractionTag.HoldingBurntFish);
        Debug.Log($"HoldingCookedFish after remove: {playerInteractionState?.CheckInteractionTag(InteractionTag.HoldingCookedFish)}");

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
