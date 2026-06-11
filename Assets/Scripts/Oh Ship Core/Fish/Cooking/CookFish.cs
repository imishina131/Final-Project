using System.Collections;
using UnityEngine;
using System;

public class CookFish : MonoBehaviour, IInteractable
{
    Material material;
    float cookedAmount;
    bool isCooking = false;
    bool isReady = false;
    bool isBurnt = false;
    bool isBurning = false;
    bool invoked = false;


    [SerializeField] HungerAndThirst stats;
    InteractionSession m_currentInteractionSession;
    HungerAndThirst hungerAndThirst;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<Renderer>().material;
        cookedAmount = material.GetFloat("_Cooked_Amount");
        Debug.Log(cookedAmount);
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForStove();

        if(isCooking)
        {
            Debug.Log("Cooking");
            StartCooking();
        }
    }


    private void StartCooking()
    {
        if(cookedAmount < 0.5f)
        {
            Debug.Log("Change Color");
            ChangeColor();
        }
        else if(cookedAmount >= 0.5f)
        {
            isCooking = false;
            isReady = true;
            if(!isBurning)
            {
                if(!invoked)
                {
                    invoked = true;
                    StartCoroutine(Burn());
                }
            }
            else if(isBurning)
            {
                ChangeColor();
            }
        }

        if (cookedAmount >= 1)
        {
            isBurnt = true;
            isReady = false;
            isBurning = false;
        }
    }

    void ChangeColor()
    {
        cookedAmount += 0.1f * Time.deltaTime;
        material.SetFloat("_Cooked_Amount", cookedAmount);
    }

    void EndCooking()
    {
        isReady = false;
        isBurnt = false;
        isBurning = false;
        invoked = false;
        gameObject.SetActive(false);
        material.SetFloat("_Cooked_Amount", 0f);
        cookedAmount = 0f;
    }


    private void CheckForStove()
    {
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;

        RaycastHit hit;

        Debug.DrawRay(origin, direction * 0.1f, Color.red);

        if (Physics.Raycast(origin, direction, out hit, 0.1f))
        {
            if (hit.collider.CompareTag("Stove"))
            {
                if(!isCooking)
                {
                    Debug.Log("Switch to true");
                    isCooking = true;
                }
            }
        }
    }

    private IEnumerator Burn()
    {
        yield return new WaitForSeconds(3);
        isBurning = true;


    }

    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        if (interactor.IsInteracting() || m_currentInteractionSession is { IsActive: true }) return null;

        hungerAndThirst = interactor.GetAssociatedGameObject().transform.root.GetComponentInChildren<HungerAndThirst>();
        Debug.Log("Begin interaction");
        if (isBurnt)
        {
            Debug.Log("Discard Fish");
            EndCooking();
        }
        else if(isReady)
        {
            Debug.Log("Eat Fish");
            Eat();
        }
        else if(isCooking)
        {
            Debug.Log("Still Cooking");
            return null;
        }
        return m_currentInteractionSession;
    }

    private void Eat()
    {
        Debug.Log("eat");
        hungerAndThirst.Hunger.Value += 0.8f;
        EndCooking();
    
        Debug.Log("Fish Eaten");
    }

}
