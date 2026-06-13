using System.Collections.Generic;
using MatrixUtils.DependencyInjection;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] InterfaceReference<IPlayerControllable, MonoBehaviour> m_playerControllable;
    [SerializeField] Transform[] m_playerSpawnPoints;

    [Inject] private ICharacterSelectionReference m_characterSelectionReference;
    readonly Dictionary<IPlayerController, OutputChannels> m_playerOutputChannels = new();
    int m_spawnedPlayers = 1;
    public void Spawn(PlayerInput playerInput)
    {
        IPlayerController controller = playerInput.gameObject.GetComponent<IPlayerController>();
        GameObject player;
        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;
        if (SelectRandom(m_playerSpawnPoints, out Transform spawnPoint))
        {
            spawnPosition = spawnPoint.position;
            spawnRotation = spawnPoint.rotation;
        }

        player = m_characterSelectionReference == null
            ? Instantiate(m_playerControllable.UnderlyingValue.gameObject, spawnPosition, spawnRotation)
                : Instantiate(m_characterSelectionReference.GetCharacterSelectionData(controller).CharacterModelPrefab, spawnPosition, spawnRotation);

       IPlayerControllable controllable = player.GetComponent<IPlayerControllable>();
       controller.ChangeControlledEntity(controllable);
       
      
        if (!m_playerOutputChannels.TryGetValue(controller, out OutputChannels channels))
        {
            channels = (OutputChannels)(1 << m_spawnedPlayers);
            m_playerOutputChannels.Add(controller, channels);
            m_spawnedPlayers++;
        }
        playerInput.GetComponentInChildren<CinemachineBrain>().ChannelMask = channels;
        player.GetComponentInChildren<CinemachineCamera>().OutputChannel = channels;
    }

    static bool SelectRandom<T>(T[] array, out T result)
    {
        if(array is null || array.Length <= 0)
        {
            result = default;
            return false;
        }
        result = array[Random.Range(0, array.Length)];
        Debug.Log($"Selected {result}");
        return true;
    }
}
