using UnityEngine;
using Unity.Netcode;

public class AttackVisual : NetworkBehaviour
{
    [SerializeField] private GameObject swooshObject;
    private PlayerController playerController;

    [SerializeField] private Vector3 highAttackPosition = new Vector3(1f, 0.6f, 0f);
    [SerializeField] private Vector3 midAttackPosition = new Vector3(1f, 0f, 0f);
    [SerializeField] private Vector3 lowAttackPosition = new Vector3(1f, -0.4f, 0f);

    [SerializeField] private Vector3 highGuardPosition = new Vector3(0f, 0.8f, 0f);
    [SerializeField] private Vector3 midGuardPosition = new Vector3(2f, 0.3f, 0f);
    [SerializeField] private Vector3 lowGuardPosition = new Vector3(2f, -0.3f, 0f);

    private Quaternion attackRotation = Quaternion.Euler(0f, 0f, 0f);
    private Quaternion guardRotation = Quaternion.Euler(0f, 0f, 90f);
    private Quaternion highGuardRotation = Quaternion.Euler(0f, 0f, 0f);

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        if (swooshObject != null)
            swooshObject.SetActive(false);
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        if (swooshObject == null) return;
        if (playerController == null) return;

        PlayerController.CombatState state = playerController.CurrentCombatState.Value;

        switch (state)
        {
            case PlayerController.CombatState.AttackHigh:
                Show(highAttackPosition, attackRotation);
                break;
            case PlayerController.CombatState.AttackMid:
                Show(midAttackPosition, attackRotation);
                break;
            case PlayerController.CombatState.AttackLow:
                Show(lowAttackPosition, attackRotation);
                break;
            case PlayerController.CombatState.GuardHigh:
                Show(highGuardPosition, highGuardRotation);
                break;
            case PlayerController.CombatState.GuardMid:
                Show(midGuardPosition, guardRotation);
                break;
            case PlayerController.CombatState.GuardLow:
                Show(lowGuardPosition, guardRotation);
                break;
            default:
                swooshObject.SetActive(false);
                break;
        }
    }

    private void Show(Vector3 position, Quaternion rotation)
    {
        float direction = OwnerClientId == 0 ? 1f : -1f;
        Vector3 flippedPosition = new Vector3(position.x * direction, position.y, position.z);

        swooshObject.SetActive(true);
        swooshObject.transform.localPosition = flippedPosition;
        swooshObject.transform.localRotation = rotation;
    }
}