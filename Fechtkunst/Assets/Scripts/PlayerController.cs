using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Combat")]
    public float attackCooldown = 0.5f;
    private float lastAttackTime;

    public NetworkVariable<CombatState> CurrentCombatState =
        new NetworkVariable<CombatState>(CombatState.Neutral,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    private Rigidbody2D rb;

    public enum CombatState
    {
        Neutral,
        AttackHigh,
        AttackMid,
        AttackLow,
        GuardHigh,
        GuardMid,
        GuardLow,
        Dodge
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
    if (Time.timeScale == 0) return;
    if (!IsOwner) return;
    if (GameManager.Instance == null) return;
    if (!GameManager.Instance.RoundInProgress.Value) return;
    HandleMovement();
    HandleCombatInput();
    }

    private void HandleMovement()
    {
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        if (Input.GetKey(KeyCode.D)) moveInput = 1f;
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleCombatInput()
    {
        bool isGuarding = Input.GetKey(KeyCode.LeftShift);

        if (isGuarding)
        {
            if (Input.GetKey(KeyCode.UpArrow))
                CurrentCombatState.Value = CombatState.GuardHigh;
            else if (Input.GetKey(KeyCode.RightArrow))
                CurrentCombatState.Value = CombatState.GuardMid;
            else if (Input.GetKey(KeyCode.LeftArrow))
                CurrentCombatState.Value = CombatState.GuardLow;
            else
                CurrentCombatState.Value = CombatState.Neutral;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && CanAttack())
            {
                CurrentCombatState.Value = CombatState.AttackHigh;
                lastAttackTime = Time.time;
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.swingSound);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && CanAttack())
            {
                CurrentCombatState.Value = CombatState.AttackMid;
                lastAttackTime = Time.time;
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.swingSound);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && CanAttack())
            {
                CurrentCombatState.Value = CombatState.AttackLow;
                lastAttackTime = Time.time;
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.swingSound);
            }

            if (CurrentCombatState.Value != CombatState.Neutral &&
                CurrentCombatState.Value != CombatState.GuardHigh &&
                CurrentCombatState.Value != CombatState.GuardMid &&
                CurrentCombatState.Value != CombatState.GuardLow)
            {
                if (Time.time > lastAttackTime + attackCooldown)
                    CurrentCombatState.Value = CombatState.Neutral;
            }
        }
    }

    private bool CanAttack()
    {
        return Time.time > lastAttackTime + attackCooldown;
    }
}