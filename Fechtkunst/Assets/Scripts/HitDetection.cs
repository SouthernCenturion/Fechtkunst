using Unity.Netcode;
using UnityEngine;

public class HitDetection : NetworkBehaviour
{
    private PlayerController myController;
    private PlayerController opponentController;

    public float attackRange = 2f;

    private void Awake()
    {
        myController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.RoundInProgress.Value) return;
        if (opponentController == null)
        {
            FindOpponent();
            return;
        }

        CheckHit();
    }

    private void FindOpponent()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (PlayerController p in players)
        {
            if (p != myController)
            {
                opponentController = p;
                break;
            }
        }
    }

    private void CheckHit()
    {
        PlayerController.CombatState myState = myController.CurrentCombatState.Value;
        bool isAttacking = myState == PlayerController.CombatState.AttackHigh ||
                           myState == PlayerController.CombatState.AttackMid ||
                           myState == PlayerController.CombatState.AttackLow;

        Debug.Log($"isAttacking: {isAttacking}, opponent: {opponentController != null}");

        if (!isAttacking) return;

        float distance = Vector2.Distance(transform.position, opponentController.transform.position);
        Debug.Log($"Distance to opponent: {distance}, attack range: {attackRange}");

        if (distance > attackRange) return;

        PlayerController.CombatState opponentState = opponentController.CurrentCombatState.Value;
        bool isParried = IsParried(myState, opponentState);

        if (!isParried)
        {
            Debug.Log($"HIT LANDED by player {OwnerClientId}");
            ulong scoringPlayerId = OwnerClientId;
            GameManager.Instance.RegisterHitServerRpc(scoringPlayerId);
        }
        else
        {
            Debug.Log("Attack was parried!");
        }
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