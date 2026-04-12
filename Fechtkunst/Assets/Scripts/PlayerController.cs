using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Combat")]
    public float attackCooldown = 0.5f;
    private float lastAttackTime;

    // Network synced combat state
    public NetworkVariable<CombatState> CurrentCombatState = 
        new NetworkVariable<CombatState>(CombatState.Neutral,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    private Rigidbody2D rb;

    // Combat state pattern
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
    if (!IsOwner) return;
    
    // Temporary debug - remove later
    //Debug.Log($"Owner: {IsOwner}, A key: {Input.GetKey(KeyCode.A)}, D key: {Input.GetKey(KeyCode.D)}");
    
    HandleMovement();
    HandleCombatInput();
    }

    private void OnGUI()
    {
    if (!IsOwner) return;
    GUI.Label(new Rect(10, 10, 300, 20), $"My State: {CurrentCombatState.Value}");
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
            // Guard states
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
            // Attack states
            if (Input.GetKeyDown(KeyCode.UpArrow) && CanAttack())
            {
                CurrentCombatState.Value = CombatState.AttackHigh;
                lastAttackTime = Time.time;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && CanAttack())
            {
                CurrentCombatState.Value = CombatState.AttackMid;
                lastAttackTime = Time.time;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && CanAttack())
            {
                CurrentCombatState.Value = CombatState.AttackLow;
                lastAttackTime = Time.time;
            }

            // Return to neutral after attack
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