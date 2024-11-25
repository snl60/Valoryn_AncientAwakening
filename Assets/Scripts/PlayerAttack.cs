using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float meleeCooldown = 0.1f;     // Cooldown time for melee attacks
    [SerializeField] private float rangedCooldown = 0.5f; // Cooldown time for ranged attacks

    [Header("Reference")]
    [SerializeField] private Collider2D meleeCollider;     // Collider for the sword attack
    [SerializeField] private GameObject arrowPrefab;       // Arrow projectile prefab
    [SerializeField] private float meleeAttackDuration = 0.2f; // How long the melee attack is active
    [SerializeField] private Animator animator;            // Animator to trigger attack animations    
    [SerializeField] private Transform firePoint;          // Point where the arrow is spawned
    [SerializeField] private PlayerMovement playerMovement;


    private float meleeTimer = 0f;
    private float rangedTimer = 0f;
    public bool isAttacking = false;

    private void Update()
    {
        // Update cooldown timers
        meleeTimer += Time.deltaTime;
        rangedTimer += Time.deltaTime;
    }

    public void OnMeleeAttack(InputAction.CallbackContext context)
    {
        if (context.performed && meleeTimer >= meleeCooldown)
        {
            MeleeAttack();
        }
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (context.performed && rangedTimer >= rangedCooldown)
        {
            RangedAttack();
        }
    }

    private void MeleeAttack()
    {
        if (isAttacking) return;

        isAttacking = true;

        meleeTimer = 0f;

        Vector2 direction = playerMovement.GetLastDirection();

        if (direction == Vector2.up) animator.Play("player_melee_up");
        else if (direction == Vector2.down) animator.Play("player_melee_down");
        else if (direction == Vector2.right) animator.Play("player_melee_right");
        else animator.Play("player_melee_right");
    }

    private void OnMeleeAttackComplete()
    {
        isAttacking = false;
        Debug.Log("Melee attack animation finished.");
    }

    private System.Collections.IEnumerator ActivateMeleeCollider()
    {
        meleeCollider.enabled = true;
        yield return new WaitForSeconds(meleeAttackDuration);
        meleeCollider.enabled = false;
    }

    private void RangedAttack()
    {
        if (isAttacking) return;

        isAttacking= true;
        
        rangedTimer = 0f;

        Vector2 direction = playerMovement.GetLastDirection();

        if (direction == Vector2.up) animator.Play("player_shoot_up");
        else if (direction == Vector2.down) animator.Play("player_shoot_down");
        else if (direction == Vector2.right) animator.Play("player_shoot_right");
        else animator.Play("player_shoot_right");


        // Spawn the arrow projectile
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);

        // Set up the arrow (assuming it has a script to handle movement and collision)
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.SetDirection(firePoint.right * (direction.x != 0 ? Mathf.Sign(direction.x) : 1)); // Aligns with facing
        }
    }

    private void OnRangedAttackComplete()
    {
        isAttacking = false;
        Debug.Log("Ranged attack animation complete.");
    }
}