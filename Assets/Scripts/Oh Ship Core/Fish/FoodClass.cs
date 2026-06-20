using UnityEngine;

public abstract class FoodClass : MonoBehaviour, IUsableItem
{
 [SerializeField] protected SO_CookableFoodData foodData;
 
 public SO_CookableFoodData FoodData { get => foodData; set => foodData = value; }
 
 public abstract CookingProcess CookingProcess { get; }
 
 private HungerAndThirst m_HungerAndThirst;

 public void InitializeHungerAndThirst(HungerAndThirst hungerAndThirst)
 {
  m_HungerAndThirst = hungerAndThirst;
 }

 public void Use()
 {
  m_HungerAndThirst.Hunger.Value += Eat();
  Debug.Log(gameObject.name + " has been used!");
  Destroy(gameObject);
 }

 public abstract float Eat();

}
