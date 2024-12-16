using System.Collections;
using UnityEngine;

public class EnemySkeletonCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float meleeRange = 1f;
    public int meleeDamage = 10;
    public float attackCooldown = 0.01f;

    //[Header("References")]
    [SerializeField] private EnemySkeletonMovement movement;
    [SerializeField] private EnemySkeletonAnimator animator;
    [SerializeField] private EnemySkeletonCombat combat;

    [SerializeField] private Collider2D meleeColliderDown;
    [SerializeField] private Collider2D meleeColliderUp;
    [SerializeField] private Collider2D meleeColliderRight;
    [SerializeField] private Collider2D meleeColliderLeft;

    private EnemyBase enemyBase;
    private GameObject player;
    private Transform playerTransform;
    public bool canAttack = true;
    public bool IsAttacking = false;

    // Variables for Maneuvering between attacks
    public float retreatDistance = 2f;
    public float moveRadius = 2f;
    public float minDistanceFromPlayer = 1f;
    public float maxDistanceFromPlayer = 7f;
    private float elapsedTime = 0f;
    private bool isManeuvering = false;

    private void Start()
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

    private void Update()
    {
        if (isManeuvering)
        {
            elapsedTime += Time.deltaTime;
        }

        if (enemyBase.CurrentState == EnemyState.Attacking && canAttack && IsPlayerInRange())
        {
            enemyBase.ChangeState(EnemyState.Attacking);
            PerformMeleeAttack();
        }
    }

    // Is Player In Range bool method that checks whether the player is within melee range
    public bool IsPlayerInRange()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        return (distanceToPlayer <= meleeRange);
    }

    // Perform Melee Attack method
    public void PerformMeleeAttack()
    {
        if (!canAttack || enemyBase.CurrentState != EnemyState.Attacking) return;

        canAttack = false;
        IsAttacking = true;
        animator.PlayAttackAnimation();

        Debug.Log("Skeleton attacks the player!");
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

        PlayerAttack playerAttack = other.GetComponent<PlayerAttack>();

        if (playerAttack == null)
        {
            playerAttack = other.GetComponentInParent<PlayerAttack>();
        }

        if (playerAttack != null)
        {
            playerAttack.TakeDamage(meleeDamage);
        }
        else
        {
            Debug.LogError("Player does not have a PlayerAttack component!");
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

        StartCoroutine(AttackCooldown());
        StartManeuvering();

        //float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        //if (distanceToPlayer > meleeRange || !canAttack)
        //{
        //    enemyBase.ChangeState(EnemyState.Chasing);
        //}
    }

    // Attack Cooldown coroutine that creates a delay between attacks and apply damage
    private IEnumerator AttackCooldown()
    {
        Debug.Log("Attack Cooldown started.");

        yield return new WaitForSeconds(attackCooldown);
        
        canAttack = true;

        Debug.Log("Attack cooldown ended. Enemy can attack again.");
    }

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

    private void StartManeuvering()
    {
        //enemyBase.ChangeState(EnemyState.Maneuvering);
        animator.PlayIdleAnimation();
        StartCoroutine(ManeuveringRoutine());
    }

    //private IEnumerator ManeuveringRoutine()
    //{
    //    enemyBase.ChangeState(EnemyState.Maneuvering);

    //    //float maneuveringDuration = attackCooldown;
    //    float elapsedTime = 0f;

    //    Vector2 lastDirection = GetLastAttackDirection();
    //    Vector2 retreatPosition = (Vector2)transform.position - (lastDirection.normalized * retreatDistance);

    //    while (elapsedTime < attackCooldown && enemyBase.CurrentState == EnemyState.Maneuvering)
    //    {
    //        Vector2 currentTarget = retreatPosition;
    //        yield return StartCoroutine(MoveToPosition(currentTarget));

    //        Vector2 basePosition = (Vector2)transform.position;
    //        Vector2 randomPoint = basePosition + Random.insideUnitCircle * moveRadius;

    //        float distanceToPlayer = Vector2.Distance(randomPoint, playerTransform.position);

    //        if (distanceToPlayer < minDistanceFromPlayer)
    //        {
    //            randomPoint = AdjustPositionAwayFromPlayer(randomPoint);
    //        }

    //        yield return StartCoroutine(MoveToPosition(randomPoint));

    //        elapsedTime += Time.deltaTime;

    //        if (elapsedTime > attackCooldown) break;
    //    }

    //    Debug.Log("Attack cooldown complete.");
    //    //canAttack = true;

    //    if (enemyBase.CurrentState == EnemyState.Maneuvering)
    //    {
    //        if (IsPlayerInRange())
    //        {
    //            enemyBase.ChangeState(EnemyState.Attacking);
    //            Debug.Log("State changed to Attacking.");
    //        }
    //        else
    //        {
    //            enemyBase.ChangeState(EnemyState.Chasing);
    //            Debug.Log("State changed to Chasing.");
    //        }
    //    }

    //    Debug.Log("Maneuvering complete. New state: " + enemyBase.CurrentState);
    //}

    private IEnumerator ManeuveringRoutine()
    {
        Debug.Log("Maneuvering routine started.");
        enemyBase.ChangeState(EnemyState.Maneuvering);
        elapsedTime = 0f;
        isManeuvering = true;

        Vector2 retreatPosition = (Vector2)transform.position - (GetLastAttackDirection().normalized * retreatDistance);

        // Move to the retreat position
        while (elapsedTime < attackCooldown && enemyBase.CurrentState == EnemyState.Maneuvering)
        {
            if (elapsedTime > attackCooldown) break;
            
            // Move to the retreat position
            while (Vector2.Distance(transform.position, retreatPosition) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, retreatPosition, movement.chaseSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Optional randomized movement
            Vector2 randomPoint = (Vector2)transform.position + Random.insideUnitCircle * moveRadius;

            if (Vector2.Distance(randomPoint, playerTransform.position) < minDistanceFromPlayer)
            {
                randomPoint = AdjustPositionAwayFromPlayer(randomPoint);
            }

            // Move to the randomized point
            while (Vector2.Distance(transform.position, randomPoint) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, randomPoint, movement.chaseSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;

                // Ensure the maneuvering duration doesn't exceed the attackCooldown
                if (elapsedTime >= attackCooldown)
                    break;
            }
        }

        Debug.Log("Maneuvering complete. Cooldown ended.");

        // Enable attacking
        canAttack = true;

        // Transition to the next state
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= meleeRange && canAttack)
        {
            enemyBase.ChangeState(EnemyState.Attacking);
            PerformMeleeAttack();
        }
        else
        {
            enemyBase.ChangeState(EnemyState.Chasing);
        }
    }


    private Vector2 AdjustPositionAwayFromPlayer(Vector2 position)
    {
        Vector2 direction = (position - (Vector2)playerTransform.position).normalized;
        return (Vector2)playerTransform.position + direction * minDistanceFromPlayer;
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
        while(Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, step);
            yield return null;
        }
        
    }

    // Public getter for IsAttacking variable
    public bool IsCurrentlyAttacking() { return IsAttacking; }
}
