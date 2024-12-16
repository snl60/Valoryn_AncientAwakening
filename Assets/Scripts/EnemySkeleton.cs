using UnityEngine;

public class EnemySkeleton : EnemyBase
{
    private EnemySkeletonMovement movement;
    private EnemySkeletonCombat combat;
    private EnemySkeletonAnimator animator;
    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;

    private void Start()
    {
        base.Start();
        
        movement = GetComponent<EnemySkeletonMovement>();
        combat = GetComponent<EnemySkeletonCombat>();
        animator = GetComponent<EnemySkeletonAnimator>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("No GameObject with tag 'Player' was found!");
        }

        if (movement == null)
        {
            Debug.LogError("EnemySkeletonMovement script is missing!");
        }

        if (combat == null)
        {
            Debug.LogError("EnemySkeletonCombat script is missing!");
        }

        if (animator == null)
        {
            Debug.LogError("EnemySkeletonAnimator script is missing!");
        }

        ChangeState(EnemyState.Patrolling);
    }

    private void Update()
    {
        //switch (currentState)
        //{
        //    case EnemyState.Idle:
        //        animator.PlayIdleAnimation();
        //        break;
        //    case EnemyState.Patrolling:
        //        movement.PatrolArea();
        //        break;
        //    case EnemyState.Chasing:
        //        movement.ChasePlayer();
        //        break;
        //    case EnemyState.Attacking:
        //        combat.PerformMeleeAttack();
        //        break;
        //    case EnemyState.Maneuvering:
        //        animator.PlayIdleAnimation();
        //        if (combat.canAttack)
        //        {
        //            ChangeState(EnemyState.Chasing);
        //        }
        //        // Maneuvering state
        //        break;
        //    case EnemyState.Stunned:
        //        // Stun effect logic here
        //        break;
        //    case EnemyState.Dead:
        //        movement.enabled = false;
        //        combat.enabled = false;
        //        break;
        //}

        movement.UpdateLastDirection(transform.position);
        //HandleStateChange();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (CurrentHealth > 0)
        {
            ChangeState(EnemyState.Hit);
            movement.enabled = false;
            combat.enabled = false;
        }
        Debug.Log("Skeleton took damage.");
    }

    private void OnHitEnd()
    {
        Debug.Log("On Hit End event triggered.");
        
        movement.enabled = true;
        combat.enabled = true;

        combat.IsAttacking = false;
        combat.DeactivateMeleeCollider();
        combat.canAttack = true;

        if (CurrentState == EnemyState.Attacking)
        {
            Debug.LogWarning("Enemy was attacking when hit. Resetting to chasing state.");
            ChangeState(EnemyState.Chasing);
        }

        if (CurrentState != EnemyState.Dead)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > combat.meleeRange)
            {
                ChangeState(EnemyState.Chasing);
            }
            else if (combat.canAttack)
            {
                ChangeState(EnemyState.Attacking);
                combat.PerformMeleeAttack();
            }
        }
        
    }

    public override void Die()
    {
        if (currentState == EnemyState.Dead) return;

        StopAllCoroutines();
        movement.enabled = false;
        combat.enabled = false;
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        ChangeState(EnemyState.Dead);
        if (playerTransform != null && playerTransform.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }

        animator.PlayDeathAnimation();

        Debug.Log("Skeleton intiated death sequence.");
        base.Die();
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }

    //public void HandleStateChange()
    //{
    //    float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

    //    switch (currentState)
    //    {
    //        case EnemyState.Patrolling:
    //            if (distanceToPlayer < movement.detectionRadius)
    //            {
    //                ChangeState(EnemyState.Chasing);
    //            }
    //            break;

    //        case EnemyState.Chasing:
    //            if (distanceToPlayer < combat.meleeRange && combat.canAttack)
    //            {
    //                ChangeState(EnemyState.Attacking);
    //            }
    //            break;

    //        case EnemyState.Attacking:
    //            if (!combat.IsCurrentlyAttacking())
    //            {
    //                ChangeState(EnemyState.Chasing);
    //            }
    //            break;

    //        case EnemyState.Hit:
    //            if (combat.IsCurrentlyAttacking())
    //            {
    //                combat.IsAttacking = false;
    //                combat.DeactivateMeleeCollider();
    //            }
    //            break;

    //        case EnemyState.Maneuvering:
    //            if (combat.canAttack)
    //            {
    //                ChangeState(EnemyState.Chasing);
    //            }
    //            break;
    //    }

        // If in Patrolling state and player enters detection radius, chase the player
        //if (currentState == EnemyState.Patrolling)
        //{
        //    if (distanceToPlayer < movement.detectionRadius && !combat.IsCurrentlyAttacking())
        //    {
        //        enemyBase.ChangeState(EnemyState.Chasing);
        //    }
        //}

        //// If in Chasing state and the player is close enough, try to attack the player
        //if (currentState == EnemyState.Chasing)
        //{
        //    if (distanceToPlayer < combat.meleeRange && combat.canAttack && !combat.IsCurrentlyAttacking())
        //    {
        //        enemyBase.ChangeState(EnemyState.Attacking);
        //    }
        //}

        ////if (currentState == EnemyState.Attacking && combat.IsCurrentlyAttacking()) return;

        //if (currentState == EnemyState.Attacking && distanceToPlayer > combat.meleeRange && !combat.IsCurrentlyAttacking())
        //{
        //    enemyBase.ChangeState(EnemyState.Chasing);
        //}

        //if (currentState == EnemyState.Dead)
        //{
        //    enemyBase.CurrentState.Equals(EnemyState.Dead);
        //}
    //}
}
