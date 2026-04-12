using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }

    // Network variables - automatically synced across all clients
    public NetworkVariable<int> Player1Score = new NetworkVariable<int>(0);
    public NetworkVariable<int> Player2Score = new NetworkVariable<int>(0);
    public NetworkVariable<bool> RoundInProgress = new NetworkVariable<bool>(false);

    // Delegate for hit events
    public delegate void OnHitDelegate(ulong scoringPlayerId);
    public static event OnHitDelegate OnPlayerScored;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        RoundInProgress.Value = true;
    }

    public void StartRound()
    {
    if (!IsServer) return;
    if (PlayerSpawner.Instance != null)
        PlayerSpawner.Instance.RespawnPlayers();
    RoundInProgress.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RegisterHitServerRpc(ulong scoringPlayerId)
    {
        if (!RoundInProgress.Value) return;

        if (scoringPlayerId == 0)
            Player1Score.Value++;
        else
            Player2Score.Value++;

        RoundInProgress.Value = false;

        OnPlayerScored?.Invoke(scoringPlayerId);

        if (CheckMatchOver())
        {
            Debug.Log("Match over!");
        }
        else
        {
            Invoke(nameof(StartRound), 2f);
        }
    }

    public bool CheckMatchOver()
    {
        return Player1Score.Value >= 2 || Player2Score.Value >= 2;
    }
}