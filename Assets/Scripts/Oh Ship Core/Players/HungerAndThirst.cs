using MatrixUtils.GenericDatatypes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class HungerAndThirst: MonoBehaviour
{
    [FormerlySerializedAs("m_hunger")] [FormerlySerializedAs("hunger")] public Observer<float> Hunger = new(0.5f);
    [FormerlySerializedAs("m_thirst")] [FormerlySerializedAs("thirst")] public Observer<float> Thirst = new(1f);
    [FormerlySerializedAs("manager")] [SerializeField] StatusBarManager m_manager;
    private IPlayerControllable m_controllable;
    [SerializeField] float m_hungerLostPerTick = 0.01f;
    private static int numberOfPassedOutPlayers = 0;
    [SerializeField] private  bool isPassedOut = false;
    
    public bool IsPassedOut => isPassedOut;
    void Start()
    {
        Hunger.Notify();
        Thirst.Notify();
    }

    void Update()
    {
        Hunger.Value = Mathf.Clamp01(Hunger.Value - (m_hungerLostPerTick * Time.deltaTime));
        if (Hunger.Value <= 0 && !isPassedOut) PassOut();
        
        m_manager.SlowDown(Hunger.Value);
        
    }
    public void OnPlayerControllerConnected(IPlayerController controller)
    {
        if (m_manager != null) return;
       
        m_manager = controller.GetAssociatedGameObject().transform.root.GetComponentInChildren<StatusBarManager>();
        
        Hunger.AddListener(m_manager.UpdateHungerBar);
    }

    public void OnPlayerControllerDisconnected(IPlayerController controller)
    {
        //Hunger.RemoveListener(m_manager.UpdateHungerBar);
        //m_manager = null;
    }

    public void ChangeLayer(GameObject player)
    {
        player.layer = LayerMask.NameToLayer("Default");
    }

    void UpdateHungerBar(float hunger) => m_manager.UpdateHungerBar(hunger);

    public void PassOut()
    {
        numberOfPassedOutPlayers++;
        isPassedOut = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        GetComponent<Rigidbody>().isKinematic = true;
        if(numberOfPassedOutPlayers >= 2)SceneManager.LoadScene("GameOver");
    }

    public void WakeUp()
    {
        Hunger.Value = 1;
        Debug.Log("Waking Up");
        numberOfPassedOutPlayers--;
        GetComponent<Rigidbody>().isKinematic = false;
        isPassedOut = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        Debug.Log($"Layer set to: {gameObject.layer}, expected: {LayerMask.NameToLayer("Player")}");
        m_manager.ResetFade();
    }
}
