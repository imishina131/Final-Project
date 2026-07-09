using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    [SerializeField] Transform[] targetLocations;
    private NavMeshAgent agent;
    private int locationIndex = 0;

    private bool goingBack = false;
    private bool collided = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(targetLocations[locationIndex].position);
    }

    // Update is called once per frame
    void Update()
    {     
        if(agent.remainingDistance < 1f)
        {
            if(!collided)
            {
                if (goingBack)
                {
                    locationIndex--;
                }
                else
                {
                    locationIndex++;
                }

                if (locationIndex >= targetLocations.Length - 1)
                {
                    goingBack = true;
                }
                else if (locationIndex <= 0)
                {
                    goingBack = false;
                }

                agent.SetDestination(targetLocations[locationIndex].position);
            }
            else if(collided)
            {
                agent.SetDestination(targetLocations[0].position);

            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player Steam Boat")
        {
            Debug.Log("collided");
            collided = true;
        }
    }
}
