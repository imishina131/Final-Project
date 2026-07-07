using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    [SerializeField] Transform[] targetLocations;
    private NavMeshAgent agent;
    private int locationIndex = 0;

    private bool goingBack = false;
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
            agent.SetDestination(targetLocations[locationIndex].position);
            if(goingBack)
            {
                locationIndex--;
            }
            else
            {
                locationIndex++;
            }

            if(locationIndex >= targetLocations.Length - 1) 
            {
                goingBack = true;
            }
            else if(locationIndex <= 0) 
            {
                goingBack = false;
            }
        }
        
    }
}
