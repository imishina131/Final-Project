using UnityEngine;

public class CookFish : MonoBehaviour
{
    Material material;
    float timer = 0;
    float cookedAmount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<Renderer>().material;
        float cookedAmount = material.GetFloat("_Cooked_Amount");
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        cookedAmount = material.GetFloat("_Cooked_Amount");
        Debug.Log("Cook:" + cookedAmount);

        CheckCookedStatus();
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Stove")
        {
            StartCooking();
        }
    }

    private void StartCooking()
    {
        timer = 0;
        int timerInInt = (int)timer;
        cookedAmount += 0.1f * Time.deltaTime;
    }

    void EndCooking()
    {

    }

    private void CheckCookedStatus()
    {

    }
}
