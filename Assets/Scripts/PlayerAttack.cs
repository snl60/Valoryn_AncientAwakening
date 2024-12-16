using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float meleeCooldown = 0f;              // Cooldown time for melee attacks
    [SerializeField] private float rangedCooldown = 0f;             // Cooldown time for ranged attacks
    [SerializeField] private int meleeDamage = 50;

    [Header("Reference")]
    [SerializeField] private GameObject arrowPrefab;                // Arrow projectile prefab
    [SerializeField] private float meleeAttackDuration = 0.2f;      // How long the melee attack is active
    [SerializeField] private Animator animator;                     // Animator to trigger attack animations    
    [SerializeField] private Transform firePoint;                   // Point where the arrow is spawned
    [SerializeField] private PlayerMovement playerMovement;         // Movement script reference
    [SerializeField] private PlayerStats playerStats;               // PlayerStats script reference
    [SerializeField] private PlayerInputActions playerInputActions; // InputActions reference
    [SerializeField] private PlayerHUD playerHUD;                   // PlayerHUD script reference
    [SerializeField] private PlayerAttack playerAttack;

    [SerializeField] private Collider2D meleeColliderDown;
    [SerializeField] private Collider2D meleeColliderUp;
    [SerializeField] private Collider2D meleeColliderRight;
    [SerializeField] private Collider2D meleeColliderLeft;

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
        if (!context.performed || meleeTimer <= meleeCooldown || playerMovement.isDashing) return;
        
        MeleeAttack();
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (!context.performed || rangedTimer <= rangedCooldown || playerMovement.isDashing) return;

        RangedAttack();
        
    }

    private void MeleeAttack()
    {
        if (isAttacking) return;

        isAttacking = true;

        meleeTimer = 0f;

        playerMovement.moveSpeed = playerMovement.moveSpeed / 2;

        Vector2 direction = playerMovement.GetLastDirection();

        if (direction == Vector2.up) animator.Play("player_melee_up");
        else if (direction == Vector2.down) animator.Play("player_melee_down");
        else if (direction == Vector2.right) animator.Play("player_melee_right");
        else animator.Play("player_melee_right");
    }

    private void OnMeleeAttackComplete()
    {
        isAttacking = false;
        playerMovement.moveSpeed = playerMovement.moveSpeed * 2;
        Debug.Log("Melee attack animation finished.");
    }

    private async void RangedAttack()
    {
        if (isAttacking) return;

        isAttacking= true;
        
        rangedTimer = 0f;

        playerMovement.moveSpeed = playerMovement.moveSpeed / 2;

        Vector2 direction = playerMovement.GetLastDirection();
        float angle = 0f;

        if (direction == Vector2.up)
        {
            animator.Play("player_shoot_up");
            angle = -90f;
        }
        else if (direction == Vector2.down)
        {
            animator.Play("player_shoot_down");
            angle = 90f;
        }
        else if (direction == Vector2.right)
        {
            animator.Play("player_shoot_right");
            angle = 0f;
        }
        else 
        {
            animator.Play("player_shoot_right");
            angle = 180f;
        }

        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // Spawn the arrow projectile after a short delay
        await Task.Delay(400);
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, rotation);

        // Set up the arrow (assuming it has a script to handle movement and collision)
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.SetDirection(direction); // Aligns with facing
        }
    }

    private void OnRangedAttackComplete()
    {
        isAttacking = false;
        playerMovement.moveSpeed = playerMovement.moveSpeed * 2;
        Debug.Log("Ranged attack animation complete.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttacking && other.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(meleeDamage);
                Debug.Log($"Dealt {meleeDamage} damage to {other.name}.");
            }
        }
    }

    private void EnableMeleeCollider()
    {
        Vector2 direction = playerMovement.GetLastDirection();

        if (direction == Vector2.up) meleeColliderUp.enabled = true;
        else if (direction == Vector2.down) meleeColliderDown.enabled = true;
        else if (direction == Vector2.right) meleeColliderRight.enabled = true;
        else meleeColliderLeft.enabled = true;
    }

    private void DisableMeleeCollider()
    {
        meleeColliderUp.enabled = false;
        meleeColliderDown.enabled = false;
        meleeColliderRight.enabled = false;
        meleeColliderLeft.enabled = false;
    }

    public void TakeDamage(int damage)
    {
        if (playerStats == null)
        {
            Debug.Log("PlayerStats reference is missing!");
            return;
        }

        playerStats.CurrentHealth -= damage;

        playerHUD.UpdateHealth(playerStats.CurrentHealth);

        if (playerStats.CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player had died.");

        playerInputActions.enabled = false;
        playerMovement.enabled = false;
        playerAttack.enabled = false;

        if (animator != null)
        {
            animator.Play("player_death");
        }

        NotifyEnemiesOfPlayerDeath();
    }

    private void NotifyEnemiesOfPlayerDeath()
    {
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
        foreach (EnemyBase enemy in enemies)
        {
            if (enemy.CurrentState == EnemyState.Attacking || enemy.CurrentState == EnemyState.Chasing)
            {
                enemy.ChangeState(EnemyState.Patrolling);
            }
        }
    }
}
