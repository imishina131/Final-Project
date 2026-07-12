using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] InterfaceReference<IPlayerControllable, MonoBehaviour> m_playerControllable;
    [SerializeField] Transform[] m_playerSpawnPoints;
    [Inject] IInjector m_injector;
    [Inject] ICharacterSelectionDataHandler m_characterSelectionDataHandler;
    readonly Dictionary<IPlayerController, OutputChannels> m_playerOutputChannels = new();
    int m_spawnedPlayers = 1;
    private List<Transform> m_spawnPointsLeft = new();

    private void Awake()
    {
        foreach (Transform location in m_playerSpawnPoints)
        {
            m_spawnPointsLeft.Add(location);
        }
    }

    public void Spawn(PlayerInput playerInput)
    {
        Debug.Log("PlayerSpawnManager.Spawn called for Player:" + playerInput.playerIndex);
        IPlayerController controller = playerInput.gameObject.GetComponent<IPlayerController>();
        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;
        if (SelectRandom(m_spawnPointsLeft, out Transform spawnPoint))
        {
            spawnPosition = spawnPoint.position;
            spawnRotation = spawnPoint.rotation;
            m_spawnPointsLeft.Remove(spawnPoint);
        }
        m_injector.Inject(controller.GetAssociatedGameObject());
        SO_CharacterSpecificData data = null;
        m_characterSelectionDataHandler?.TryGetCharacterSelectionData(playerInput.playerIndex, out data);
        Debug.Log($"Player {playerInput.playerIndex} data: {data?.name ?? "NULL"}, CharacterModelPrefab: {data?.CharacterModelPrefab?.name ?? "NULL"}");
        IPlayerControllable selectedControllable = data?.CharacterModelPrefab.GetComponent<IPlayerControllable>() ?? m_playerControllable.Value;
        Debug.Log($"Selected controllable: {selectedControllable?.GetAssociatedGameObject()?.name ?? "NULL"}");
        GameObject player = Instantiate(selectedControllable.GetAssociatedGameObject(), spawnPosition, spawnRotation);
        controller.ChangeControlledEntity(player.GetComponent<IPlayerControllable>());
        
        controller.GetAssociatedGameObject().GetComponentInChildren<WarningIconMessageListener>()?.OnPlayerControllerConnect(controller);
        if (data != null && controller.GetAssociatedGameObject().GetComponentInChildren<UniversalAdditionalCameraData>() is { } cam && controller.GetAssociatedGameObject().GetComponentInChildren<Volume>() is { } volume)
        {
            cam.volumeLayerMask |= data.CharacterPostEffectLayer;
            volume.gameObject.layer = LayerMaskToLayer(data.CharacterPostEffectLayer);
        }
        if (!m_playerOutputChannels.TryGetValue(controller, out OutputChannels channels))
        {
            channels = (OutputChannels)(1 << m_spawnedPlayers);
            m_playerOutputChannels.Add(controller, channels);
            m_spawnedPlayers++;
        }
        playerInput.GetComponentInChildren<CinemachineBrain>().ChannelMask = channels;
        player.GetComponentInChildren<CinemachineCamera>().OutputChannel = channels;
        Debug.Log($"Player {playerInput.playerIndex} Data;{data.name ?? "NULL"}");
    }

    static bool SelectRandom<T>(List<T> list, out T result)
    {
        if(list is null || list.Count <= 0)
        {
            result = default;
            return false;
        }
        result = list[Random.Range(0, list.Count)];
        return true;
    }
    [Pure]
    static int LayerMaskToLayer(LayerMask mask)
    {
        int bitmask = mask.value;
        for (int i = 0; i < 32; i++)
        {
            if ((bitmask & (1 << i)) != 0)
            {
                return i;
            }
        }
        return 0;
    }
}
