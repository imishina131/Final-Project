using UnityEngine;

public class AISpawner : MonoBehaviour
{
    [SerializeField] GameObject aiPrefab;
    [SerializeField] Transform[] targetLocations;

    AIMovement aiLogic;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if(aiLogic == null)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        GameObject spawnedAI = Instantiate(aiPrefab, gameObject.transform.position, gameObject.transform.rotation);
        aiLogic = spawnedAI.GetComponent<AIMovement>();
        aiLogic.targetLocations = targetLocations;
    }
}
