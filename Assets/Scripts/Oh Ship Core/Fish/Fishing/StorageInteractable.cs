using System.Collections.Generic;
using UnityEngine;

public class StorageInteractable : MonoBehaviour, IInteractable
{
    private IPlayerControllable _playerControllable;
    private IPlayerController _playerController;
    private Transform _holdingObjectTransform;
    InteractionSession m_currentInteractionSession;
    public int _storedFish = 0;
    [SerializeField] private int maxStoredFish = 5;
    [SerializeField] GameObject fishPrefab;
    private PlayerInteractionState _playerInteractionState;
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        _playerControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
       
        _playerController = _playerControllable.GetActivePlayerController();
        
        _playerInteractionState = _playerControllable.GetAssociatedGameObject().GetComponent<PlayerInteractionState>();

        
        if (_storedFish < maxStoredFish && _playerInteractionState.CheckInteractionTag(InteractionTag.Holding))
        {
            Debug.Log("Add fish to container");
            _holdingObjectTransform =  _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectLocation>().transform;
            
            AddFishToStorage();
            Destroy(_holdingObjectTransform.GetChild(0).gameObject);
            m_currentInteractionSession = new InteractionSession(interactor,this);
            _playerInteractionState.RemoveInteractionTag(InteractionTag.Holding);
            m_currentInteractionSession.End();
            return m_currentInteractionSession;
        }

        if (_storedFish == 0)
        {
            m_currentInteractionSession = new InteractionSession(interactor, this);
            m_currentInteractionSession.End();
            return m_currentInteractionSession;
        }
        m_currentInteractionSession = new InteractionSession(interactor, this);
        
        RemoveFishFromStorage(fishPrefab);
        
        return m_currentInteractionSession;
    }
    
    public void AddFishToStorage()
    {
        _storedFish++;
    }

    public void RemoveFishFromStorage(GameObject fishRef)
    {
        HungerAndThirst hungerRef = _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HungerAndThirst>();
        _storedFish--;
        _holdingObjectTransform =  _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectLocation>().transform;
        GameObject fish = Instantiate(fishRef, _holdingObjectTransform.position,_holdingObjectTransform.rotation);
        Debug.Log(fish.GetComponent<Fish>().CurrentCookState);
        fish.transform.SetParent(_holdingObjectTransform);
        fish.GetComponent<FoodClass>().InitializeHungerAndThirst(hungerRef);
        _playerInteractionState.AddInteractionTag(InteractionTag.Holding);
    }

    
}
