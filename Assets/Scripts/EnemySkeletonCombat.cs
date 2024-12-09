using System.Collections;
using UnityEngine;

public class EnemySkeletonCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float meleeRange = 1f;
    public int meleeDamage = 10;
    public float attackCooldown = 3f;

    //[Header("References")]
    [SerializeField] private EnemySkeletonMovement movement;
    [SerializeField] private EnemySkeletonAnimator animator;

    [SerializeField] private Collider2D meleeColliderDown;
    [SerializeField] private Collider2D meleeColliderUp;
    [SerializeField] private Collider2D meleeColliderRight;
    [SerializeField] private Collider2D meleeColliderLeft;

    private EnemyBase enemyBase;
    private GameObject player;
    private Transform playerTransform;
    public bool canAttack = true;
    public bool IsAttacking = false;

    // Variables for Strategic Movement method
    public float retreatDistance = 2f;
    public float moveRadius = 3f;
    public float minDistanceFromPlayer = 1f;
    public float maxDistanceFromPlayer = 7f;

    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj;
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("No Player GameObject found!");
        }
    }

    void Update()
    {
        // attackTimer += Time.deltaTime;

        if (enemyBase.CurrentState == EnemyState.Attacking && canAttack && IsPlayerInRange())
        {
            PerformMeleeAttack();
        }
    }

    // Is Player In Range bool method that checks whether the player is within melee range
    private bool IsPlayerInRange()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        return (distanceToPlayer <= meleeRange);
    }

    // Perform Melee Attack method
    public void PerformMeleeAttack()
    {
        //if (
        //    enemyBase.CurrentState != EnemyState.Attacking ||
        //    attackTimer < attackCooldown ||
        //    IsAttacking || 
        //    !IsPlayerInRange()
        //) return;

        if (!canAttack || enemyBase.CurrentState == EnemyState.Dead) return;

        canAttack = false;
        // ActivateMeleeCollider(movement.CurrentDirection);

        //Vector2 previousPosition = transform.position;
        //movement.UpdateLastDirection(previousPosition);

        animator.PlayAttackAnimation();

        Debug.Log("Skeleton attacks the player!");

        StartCoroutine(AttackCooldown());
    }

    // Activate Melee Collider method that activates the correct melee collider based LastDirection
    private void ActivateMeleeCollider(string direction)
    {
        meleeColliderDown.enabled = false;
        meleeColliderUp.enabled = false;
        meleeColliderRight.enabled = false;
        meleeColliderLeft.enabled = false;

        switch (direction)
        {
            case "down":
                meleeColliderDown.enabled = true;
                break;
            case "up":
                meleeColliderUp.enabled = true;
                break;
            case "right":
                meleeColliderRight.enabled = true;
                break;
            case "left":
                meleeColliderLeft.enabled = true;
                break;
        }
    }

    // On Trigger Enter method that detects whether the melee collider collides with the player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsAttacking || other.tag != "Player") return;

        Debug.Log("Melee attack hits the player!");

        PlayerStats playerStats = other.GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            playerStats = other.GetComponentInParent<PlayerStats>();
        }

        if (playerStats != null)
        {
            playerStats.TakeDamage(meleeDamage);
            // StartCoroutine(DamageCooldown());
        }
        else
        {
            Debug.LogError("Player does not have a PlayerStats component!");
        }
    }

    // Animation Event that triggers on the first frame of the melee attack animations
    public void OnAttackStart()
    {
        IsAttacking = true;
    }

    // Animator Event that triggers on the last frame of melee attack animations
    public void OnAttackEnd()
    {
        Debug.Log("Skeleton attack completed.");
        IsAttacking = false;
        DeactivateMeleeCollider();
        animator.PlayIdleAnimation();

        StartManeuvering();
        // StartCoroutine(AttackCooldown());
    }

    // Attack Cooldown subroutine that creates a delay between attacks and apply damage
    private IEnumerator AttackCooldown()
    {
        Debug.Log("Attack Cooldown started.");
        yield return new WaitForSeconds(attackCooldown);
        Debug.Log("Attack Cooldown ended.");
        if (enemyBase.CurrentState != EnemyState.Dead)
        {
            canAttack = true;
            Debug.Log("Skeleton can attack again.");
        }
        
        //if (player != null)
        //{

        //    float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        //    if (distanceToPlayer > meleeRange)
        //    {
        //        enemyBase.ChangeState(EnemyState.Chasing);
        //    }
        //    else if (distanceToPlayer > movement.detectionRadius)
        //    {
        //        enemyBase.ChangeState(EnemyState.Patrolling);
        //    }
        //    else
        //    {
        //        enemyBase.ChangeState(EnemyState.Attacking);
        //    }
        //}
        //else
        //{
        //    enemyBase.ChangeState(EnemyState.Patrolling);
        //}
    }

    // Damage Cooldown subroutine that ensures the player isn't damaged multiple times in one attack
    //private IEnumerator DamageCooldown()
    //{
    //    canTakeDamage = false;
    //    yield return new WaitForSeconds(attackCooldown);
    //    canTakeDamage = true;
    //}

    // Animator Event that triggers on the 4th frame of melee attack animation
    public void ApplyDamage()
    {
        if (player == null) return;
        ActivateMeleeCollider(movement.CurrentDirection);
        Debug.Log("Melee collider activated.");
    }

    // Deactivate Melee Collider method that disables all melee colliders
    public void DeactivateMeleeCollider()
    {
        meleeColliderDown.enabled = false;
        meleeColliderUp.enabled = false;
        meleeColliderRight.enabled = false;
        meleeColliderLeft.enabled = false;
    }

    public void StartManeuvering()
    {
        enemyBase.ChangeState(EnemyState.Maneuvering);
        StartCoroutine(ManeuveringRoutine());
    }

    private IEnumerator ManeuveringRoutine()
    {
        Vector2 lastDirection = GetLastAttackDirection();
        Vector2 retreatPosition = (Vector2)transform.position - (lastDirection.normalized * retreatDistance);
        Vector2 currentTarget = retreatPosition;
        yield return MoveToPosition(currentTarget);

        while (!canAttack && enemyBase.CurrentState != EnemyState.Dead)
        {
            Vector2 basePosition = (Vector2)transform.position;
            Vector2 randomPoint = basePosition + Random.insideUnitCircle * moveRadius;

            float distanceToPlayer = Vector2.Distance(randomPoint, playerTransform.position);

            if (distanceToPlayer < minDistanceFromPlayer)
            {
                Vector2 awayFromPlayer = (randomPoint - (Vector2)playerTransform.position).normalized;
                randomPoint = (Vector2)playerTransform.position + awayFromPlayer * minDistanceFromPlayer;
            }
            else if (distanceToPlayer > maxDistanceFromPlayer)
            {
                Vector2 towardsPlayer = ((Vector2)playerTransform.position - randomPoint).normalized;
                randomPoint = (Vector2)playerTransform.position + towardsPlayer * maxDistanceFromPlayer;
            }

            currentTarget = randomPoint;
            yield return MoveToPosition(currentTarget);

            //yield return new WaitForSeconds(1f);
        }

        enemyBase.ChangeState(EnemyState.Chasing);
    }

    private Vector2 GetLastAttackDirection()
    {
        switch (movement.CurrentDirection)
        {
            case "up": return Vector2.up;
            case "down": return Vector2.down;
            case "left": return Vector2.left;
            case "right": return Vector2.right;
            default: return Vector2.down;
        }
    }

    private IEnumerator MoveToPosition(Vector2 targetPos)
    {
        float step = movement.chaseSpeed * Time.deltaTime;
        while(Vector2.Distance(transform.position, targetPos) > 0.1f && !canAttack)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, step);
            yield return null;
        }
        
    }

    // Public getter for IsAttacking variable
    public bool IsCurrentlyAttacking() { return IsAttacking; }
}
