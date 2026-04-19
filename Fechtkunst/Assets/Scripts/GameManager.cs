using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public NetworkVariable<int> Player1Score = new NetworkVariable<int>(0);
    public NetworkVariable<int> Player2Score = new NetworkVariable<int>(0);
    public NetworkVariable<bool> RoundInProgress = new NetworkVariable<bool>(false);
    public NetworkVariable<ulong> WinnerClientId = new NetworkVariable<ulong>(0);

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

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.hitSound);

        if (PlayerSpawner.Instance != null)
            PlayerSpawner.Instance.RespawnPlayers();

        if (CheckMatchOver())
        {
            WinnerClientId.Value = scoringPlayerId;

            int playerNumber = 1;
            int index = 0;
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                index++;
                if (client.ClientId == scoringPlayerId)
                {
                    playerNumber = index;
                    break;
                }
            }
            string winnerName = $"Player {playerNumber}";
            SetWinnerClientRpc(winnerName);

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.PlayerObject != null)
                    client.PlayerObject.Despawn();
            }
            NetworkManager.Singleton.SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
        }
        else
        {
            Invoke(nameof(StartRound), 2f);
        }
    }

    [ClientRpc]
    private void SetWinnerClientRpc(string winnerName)
    {
        if (DatabaseManager.Instance != null)
            DatabaseManager.Instance.LastWinnerName = winnerName;
    }

    public bool CheckMatchOver()
    {
        return Player1Score.Value >= 2 || Player2Score.Value >= 2;
    }
}