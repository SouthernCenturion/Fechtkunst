using Unity.Netcode;
using UnityEngine;

public class HitDetection : NetworkBehaviour
{
    private PlayerController myController;
    public float attackRange = 2f;

    private void Awake()
    {
        myController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        if (!IsOwner) return;
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.RoundInProgress.Value) return;
        CheckAttackInput();
    }

    private void CheckAttackInput()
    {
        PlayerController.CombatState myState = myController.CurrentCombatState.Value;
        bool isAttacking =
            myState == PlayerController.CombatState.AttackHigh ||
            myState == PlayerController.CombatState.AttackMid ||
            myState == PlayerController.CombatState.AttackLow;
        if (!isAttacking) return;
        RequestHitCheckServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestHitCheckServerRpc(ServerRpcParams rpcParams = default)
    {
        PlayerController opponent = GetOpponent();
        if (opponent == null) return;

        float distance = Vector2.Distance(transform.position, opponent.transform.position);
        if (distance > attackRange) return;

        PlayerController.CombatState myState = myController.CurrentCombatState.Value;
        PlayerController.CombatState opponentState = opponent.CurrentCombatState.Value;

        bool isParried = IsParried(myState, opponentState);
        if (!isParried)
        {
            PlayAttackSoundClientRpc();
            GameManager.Instance.RegisterHitServerRpc(OwnerClientId);
        }
        else
        {
            PlayParrySoundClientRpc();
        }
    }

    [ClientRpc]
    private void PlayAttackSoundClientRpc()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.attackSound);
    }

    [ClientRpc]
    private void PlayParrySoundClientRpc()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.parrySound);
    }

    private PlayerController GetOpponent()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (PlayerController p in players)
        {
            if (p.OwnerClientId != OwnerClientId)
                return p;
        }
        return null;
    }

    private bool IsParried(PlayerController.CombatState attack, PlayerController.CombatState defense)
    {
        if (attack == PlayerController.CombatState.AttackHigh &&
            defense == PlayerController.CombatState.GuardHigh) return true;
        if (attack == PlayerController.CombatState.AttackMid &&
            defense == PlayerController.CombatState.GuardMid) return true;
        if (attack == PlayerController.CombatState.AttackLow &&
            defense == PlayerController.CombatState.GuardLow) return true;
        return false;
    }
}