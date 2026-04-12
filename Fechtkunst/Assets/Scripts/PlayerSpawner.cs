using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public static PlayerSpawner Instance { get; private set; }

    public Transform spawnPoint1;
    public Transform spawnPoint2;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        Invoke(nameof(MovePlayersToSpawnPoints), 0.1f);
    }

    private void OnClientConnected(ulong clientId)
    {
        Invoke(nameof(MovePlayersToSpawnPoints), 0.1f);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    public void RespawnPlayers()
    {
        if (!IsServer) return;
        Invoke(nameof(MovePlayersToSpawnPoints), 0.1f);
    }

    private void MovePlayersToSpawnPoints()
    {
        int index = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
            {
                if (index == 0)
                    client.PlayerObject.transform.position = spawnPoint1.position;
                else
                    client.PlayerObject.transform.position = spawnPoint2.position;
                index++;
            }
        }
    }
}