using System;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public abstract class FoodClass : MonoBehaviour, IHeldItem
{
    [FormerlySerializedAs("foodData")] [SerializeField] protected SO_CookableFoodData m_foodData;
    
    [SerializeField]  private AudioClip chewingSound;
    public SO_CookableFoodData FoodData => m_foodData;

    public abstract CookingProcess CookingProcess { get; }

    public abstract CookState CookStateRef { get; }

    HungerAndThirst m_hungerAndThirst;
    private AudioSource m_audioSource;
    
    protected PlayerInteractionState playerInteractionState;

    public void InitializeHungerAndThirst(HungerAndThirst hungerAndThirst)
    {
        m_hungerAndThirst = hungerAndThirst;
        m_audioSource = m_hungerAndThirst.GetComponentInChildren<AudioSource>();
        playerInteractionState = m_hungerAndThirst.GetComponent<PlayerInteractionState>();
        //Reset();
    }

    public void Use()
    {
        m_hungerAndThirst.Hunger.Value += Eat();
        m_audioSource.PlayOneShot(chewingSound);
        Debug.Log(gameObject.name + " has been used!");
        Destroy(gameObject);
    }

    public Transform GetTransform() => transform;
    public virtual Vector3 GetPositionOffset() => Vector3.zero;
    public virtual Quaternion GetRotationOffset() => Quaternion.identity;
    public float GetCookingSpeed() => FoodData.CookSpeed;
    public virtual void UpdateCookedAmount(float amount) { }
    public virtual void Reset() { }
    public abstract float Eat();
}
