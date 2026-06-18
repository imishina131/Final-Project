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
    
    public InteractionSession BeginInteraction(IInteractor interactor)
    {
        _playerControllable = interactor.GetAssociatedGameObject().transform.parent.GetComponent<IPlayerControllable>();
       
        _playerController = _playerControllable.GetActivePlayerController();
        
        if (_storedFish < maxStoredFish && interactor.WasInteracting)
        {
            Debug.Log("Add fish to container");
            _holdingObjectTransform =  _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectLocation>().transform;
            
            AddFishToStorage();
            Destroy(_holdingObjectTransform.GetChild(0).gameObject);
            m_currentInteractionSession = new InteractionSession(interactor,this);
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
        _storedFish--;
        _holdingObjectTransform =  _playerControllable.GetAssociatedGameObject().GetComponentInChildren<HeldObjectLocation>().transform;
        GameObject fish = Instantiate(fishRef, _holdingObjectTransform.position,_holdingObjectTransform.rotation);
        fish.transform.SetParent(_holdingObjectTransform);
    }
}
