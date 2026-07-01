using MatrixUtils.GenericDatatypes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class HungerAndThirst: MonoBehaviour
{
    [FormerlySerializedAs("m_hunger")] [FormerlySerializedAs("hunger")] public Observer<float> Hunger = new(0.5f);
    [FormerlySerializedAs("m_thirst")] [FormerlySerializedAs("thirst")] public Observer<float> Thirst = new(1f);
    [FormerlySerializedAs("manager")] private HungerAndThirstVisualManager m_manager;
    [SerializeField] private SerializableDictionary<InteractionTag, float> m_hungerLossRates;
    [SerializeField] private float thirstLossRate = 0.01f;
    static int numberOfPassedOutPlayers;
    [FormerlySerializedAs("isPassedOut")] [SerializeField] bool m_isPassedOut;
    [SerializeField] VisualEffect m_passedOutEffect;
    [SerializeField] private string shipTag;
     private PlayerInteractionState m_playerInteractionState;
     private PlayerInteractor m_playerInteractor;

    private bool fromHunger = false;
    private bool fromThirst = false;
    public bool IsPassedOut => m_isPassedOut;
    [SerializeField] private UnityEvent<bool> OnEnableMovement = new UnityEvent<bool>();


    [SerializeField] private Animator anim;
    void Start()
    {
         m_playerInteractor = GetComponentInChildren<PlayerInteractor>();
         m_playerInteractionState = GetComponent<PlayerInteractionState>();
        
        Hunger.Notify();
        Thirst.Notify();
    }

    void Update()
    {
        foreach (var hungerChecks in m_hungerLossRates)
        {
            if (m_playerInteractionState.CheckInteractionTag(hungerChecks.Key))
            {
                Hunger.Value = Mathf.Clamp01(Hunger.Value - (hungerChecks.Value * Time.deltaTime));
            }
        }

        Thirst.Value = Mathf.Clamp01(Thirst.Value - thirstLossRate * Time.deltaTime);

        if (Hunger.Value <= 0 && !m_isPassedOut) PassOut(1);
        if (Thirst.Value <= 0 && !m_isPassedOut) PassOut(2);
    }
    public void OnPlayerControllerConnected(IPlayerController controller)
    {
        if (m_manager != null) return;
        m_manager = controller.GetAssociatedGameObject().transform.root.GetComponentInChildren<HungerAndThirstVisualManager>();
        Hunger.AddListener(m_manager.UpdateHunger);
        Thirst.AddListener(m_manager.UpdateThirst);

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

    //void UpdateHungerBar(float hunger) => m_manager.UpdateHunger(hunger);

    public void PassOut(int cause)
    {
        if(cause == 1)
        {
            fromHunger = true;
        }
        else if(cause == 2)
        {
            fromThirst = true;
        }
        anim.SetBool("Faints", true);
        m_playerInteractor.EndActiveInteraction();
        numberOfPassedOutPlayers++;
        m_isPassedOut = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        OnEnableMovement.Invoke(false);
        if(numberOfPassedOutPlayers >= 2)SceneManager.LoadScene("GameOver");
        m_passedOutEffect.Play();
    }

    public void WakeUp(float value)
    {
        if(fromHunger)
        {
            Hunger.Value = value;
            fromHunger = false;
        }
        else if (fromThirst)
        {
            Thirst.Value = value;
            fromThirst = false;
        }

        anim.SetBool("Faints", false);
        Debug.Log("Waking Up");
        numberOfPassedOutPlayers--;
        GetComponent<Rigidbody>().isKinematic = false;
        m_isPassedOut = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        Debug.Log($"Layer set to: {gameObject.layer}, expected: {LayerMask.NameToLayer("Player")}");
        m_passedOutEffect.Stop();
        OnEnableMovement.Invoke(true);

    }
}
