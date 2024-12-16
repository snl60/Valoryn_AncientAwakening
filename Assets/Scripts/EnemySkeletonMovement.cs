using System.Collections.Concurrent;
using UnityEngine;

public class EnemySkeletonMovement : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolRadius = 5f;
    public float patrolSpeed = 2f;
    public float pauseDuration = 3f;

    [Header("Chase Settings")]
    public float chaseSpeed = 3.5f;
    public float detectionRadius = 7f;

    [Header("References")]
    //[SerializeField] private Animator animator;
    //[SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private EnemySkeletonCombat combat;
    [SerializeField] private EnemySkeletonAnimator animator;

    private Transform playerTransform;
    private Vector2 initialPosition;
    private Vector2 patrolTarget;
    private bool isWaiting = false;
    private string currentDirection = "down";
    private EnemyBase enemyBase;

    public string CurrentDirection => currentDirection;

    public Vector2 LastDirection { get; private set; } = Vector2.zero;

    private void Start()
    {
        initialPosition = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        SetNewPatrolTarget();

        enemyBase = GetComponent<EnemyBase>();
    }

    public void PatrolArea()
    {
        if (isWaiting) return;

        Vector2 previousPosition = transform.position;

        enemyBase.speed = patrolSpeed;

        transform.position = Vector2.MoveTowards(
            transform.position,
            patrolTarget,
            patrolSpeed * Time.deltaTime
        );

        UpdateLastDirection(previousPosition);

        animator.PlayWalkingAnimation();
        
        if (Vector2.Distance(transform.position, patrolTarget) < 0.1f)
        {
            isWaiting = true;
            animator.PlayIdleAnimation();
            Invoke(nameof(SetNewPatrolTarget), pauseDuration);
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < detectionRadius)
        {
            enemyBase.ChangeState(EnemyState.Chasing);
        }
    }

    private void SetNewPatrolTarget()
    {
        isWaiting = false;
        Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;
        patrolTarget = initialPosition + randomPoint;
    }

    public void ChasePlayer()
    {
        if (enemyBase.CurrentState != EnemyState.Chasing || playerTransform == null) return;

        enemyBase.speed = chaseSpeed;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > combat.meleeRange && !combat.IsCurrentlyAttacking())
        {
            Vector2 previousPosition = transform.position;

            transform.position = Vector2.MoveTowards(
                transform.position,
                playerTransform.position,
                chaseSpeed * Time.deltaTime
            );

            UpdateLastDirection(previousPosition);

            animator.PlayWalkingAnimation();
        }
        else
        {
            enemyBase.ChangeState(EnemyState.Attacking);
        }
    }

    public void UpdateLastDirection(Vector2 previousPosition)
    {
        if (combat.IsCurrentlyAttacking()) return;

        Vector2 movementDirection;

        if (playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) <= combat.meleeRange)
        {
            movementDirection = (playerTransform.position - transform.position).normalized;
        }
        else
        {
            movementDirection = ((Vector2)transform.position - previousPosition).normalized;
        }

        if (movementDirection == Vector2.zero) return;
        
        // Check for the dominant axis and direction to determine correct animation to play
        if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.y))
        {
            currentDirection = movementDirection.x > 0 ? "right" : "left";
        }
        else
        {
            currentDirection = movementDirection.y > 0 ? "up" : "down";
        }

        LastDirection = movementDirection;
    }

    //private void CheckForPlayer()
    //{
    //    if (playerTransform == null || enemyBase.CurrentState == EnemyState.Attacking) return;

    //    float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

    //    if (distanceToPlayer <= combat.meleeRange || distanceToPlayer > detectionRadius)
    //    {
    //        enemyBase.ChangeState(EnemyState.Attacking);
    //    }
    //    else
    //    {
    //        enemyBase.ChangeState(EnemyState.Chasing);
    //    }
    //}
}
