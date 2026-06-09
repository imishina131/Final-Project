using System.Collections;
using UnityEngine;
using System;

public class CookFish : MonoBehaviour
{
    Material material;
    float cookedAmount;
    bool isCooking = false;
    bool isReady = false;
    bool isBurnt = false;
    [SerializeField] Stats stats;
    [SerializeField] SimpleStatModifier modifier;
    [SerializeField] StatData statToModify;

    InteractionSession m_currentInteractionSession;

    StatBroker mediator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<Renderer>().material;
        cookedAmount = material.GetFloat("_Cooked_Amount");
    }

    // Update is called once per frame
    void Update()
    {
        CheckForStove();

        if(isCooking)
        {
            StartCooking();
        }
    }


    private void StartCooking()
    {
        if(cookedAmount < 0.7f)
        {
            cookedAmount += 0.1f * Time.deltaTime;
            material.SetFloat("_Cooked_Amount", cookedAmount);
        }

        if(cookedAmount >= 0.7f)
        {
            isCooking = false;
            isReady = true;
            StartCoroutine(Burn());
        }
    }

    void EndCooking()
    {
        isReady = false;
        isBurnt = true;
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
                    isCooking = true;
                }
            }
        }
    }

    private IEnumerator Burn()
    {
        yield return new WaitForSeconds(3);
        cookedAmount += 0.1f * Time.deltaTime;
        material.SetFloat("_Cooked_Amount", cookedAmount);

        if(cookedAmount >= 1)
        {
            EndCooking();
        }

    }

    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        if (interactor.IsInteracting() || m_currentInteractionSession is { IsActive: true }) return null;

        if (isBurnt)
        {
            Discard();
        }
        else if(isReady)
        {
            Eat();
        }
        else if(isCooking)
        {
            return null;
        }
        return m_currentInteractionSession;
    }

    private void Eat()
    {
        //sets stats
        Func<float, float> Add = (x) => x + 5;
        modifier = new SimpleStatModifier(Add, statToModify);
        mediator = stats.broker;
        mediator.AddModifier(modifier);
        isReady = false;
        gameObject.SetActive(false);

    }

    private void Discard()
    {
        gameObject.SetActive(false);
        material.SetFloat("_Cooked_Amount", 0f);
        cookedAmount = 0f;
    }

}
