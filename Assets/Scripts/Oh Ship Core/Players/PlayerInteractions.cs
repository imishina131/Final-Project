using Unity.Multiplayer.PlayMode;
using UnityEngine;
using static Codice.Client.Common.EventTracking.TrackFeatureUseEvent.Features.DesktopGUI.Filters;

public class PlayerInteractions : MonoBehaviour
{
    private string currentPlayer = "";
    [SerializeField] private Transform spawnPoint;

    bool inSceneA = false;
    bool inSceneB = false;
    Rigidbody boatRb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        SetPlayers();
        inSceneA = true;
        Spawn("SpawnSceneA");
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetPlayers()
    {
        if (GameObject.FindGameObjectsWithTag("Player01").Length > 0)
        {
            gameObject.tag = "Player02";
        }
        else if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
        {
            gameObject.tag = "Player01";
        }

        currentPlayer = gameObject.tag;
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckDoorEntry(other);
    }

    void Spawn(string spawnTag)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;

        spawnPoint = GameObject.FindWithTag(spawnTag).transform;
        rb.position = spawnPoint.position;

        GameObject boatObj = GameObject.FindWithTag("Boat");
        boatRb = boatObj.GetComponent<Rigidbody>();

        FixedJoint joint = boatObj.AddComponent<SpringJoint>();

        joint.connectedBody = rb;
    }

    void CheckDoorEntry(Collider door)
    {
        if (door.gameObject.tag == "Bottom")
        {

            Spawn("SpawnSceneB");
        }

        if (door.gameObject.tag == "Top")
        {

            Spawn("SpawnSceneA");
        }
    }

}
