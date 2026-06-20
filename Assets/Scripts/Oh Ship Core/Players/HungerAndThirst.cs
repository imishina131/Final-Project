using MatrixUtils.GenericDatatypes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class HungerAndThirst: MonoBehaviour
{
    [FormerlySerializedAs("m_hunger")] [FormerlySerializedAs("hunger")] public Observer<float> Hunger = new(0.5f);
    [FormerlySerializedAs("m_thirst")] [FormerlySerializedAs("thirst")] public Observer<float> Thirst = new(1f);
    [FormerlySerializedAs("manager")] [SerializeField] HungerAndThirstVisualManager m_manager;
    IPlayerControllable m_controllable;
    [SerializeField] float m_hungerLostPerTick = 0.01f;
    static int numberOfPassedOutPlayers;
    [FormerlySerializedAs("isPassedOut")] [SerializeField] bool m_isPassedOut;
    [SerializeField] VisualEffect m_passedOutEffect;
    public bool IsPassedOut => m_isPassedOut;
    void Start()
    {
        Hunger.Notify();
        Thirst.Notify();
    }

    void Update()
    {
        Hunger.Value = Mathf.Clamp01(Hunger.Value - (m_hungerLostPerTick * Time.deltaTime));
        if (Hunger.Value <= 0 && !m_isPassedOut) PassOut();
    }
    public void OnPlayerControllerConnected(IPlayerController controller)
    {
        if (m_manager != null) return;
        m_manager = controller.GetAssociatedGameObject().transform.root.GetComponentInChildren<HungerAndThirstVisualManager>();
        Hunger.AddListener(m_manager.UpdateHunger);
    }

    public void OnPlayerControllerDisconnected(IPlayerController controller)
    {
        //Hunger.RemoveListener(m_manager.UpdateHunger);
        //m_manager = null;
    }

    public void ChangeLayer(GameObject player)
    {
        player.layer = LayerMask.NameToLayer("Default");
    }

    void UpdateHungerBar(float hunger) => m_manager.UpdateHunger(hunger);

    public void PassOut()
    {
        numberOfPassedOutPlayers++;
        m_isPassedOut = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        GetComponent<Rigidbody>().isKinematic = true;
        if(numberOfPassedOutPlayers >= 2)SceneManager.LoadScene("GameOver");
        m_passedOutEffect.Play();
    }

    public void WakeUp()
    {
        Hunger.Value = 1;
        Debug.Log("Waking Up");
        numberOfPassedOutPlayers--;
        GetComponent<Rigidbody>().isKinematic = false;
        m_isPassedOut = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        Debug.Log($"Layer set to: {gameObject.layer}, expected: {LayerMask.NameToLayer("Player")}");
        m_passedOutEffect.Stop();
    }
}
