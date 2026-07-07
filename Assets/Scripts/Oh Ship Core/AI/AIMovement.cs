using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    [SerializeField] Transform[] targetLocations;
    private NavMeshAgent agent;
    private int locationIndex = 0;
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
            locationIndex++;
            agent.SetDestination (targetLocations[locationIndex].position);

            if(locationIndex >= targetLocations.Length - 1) 
            {
                locationIndex = 0;
            }
        }
        
    }
}
