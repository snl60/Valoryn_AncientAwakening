using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class EnemySkeletonStateController : MonoBehaviour
{
    [SerializeField] EnemyBase enemyBase;
    [SerializeField] EnemySkeleton skeleton;
    [SerializeField] EnemySkeletonMovement movement;
    [SerializeField] EnemySkeletonCombat combat;
    [SerializeField] EnemySkeletonAnimator animator;
    
    private Transform playerTransform;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        switch (enemyBase.CurrentState)
        {
            case EnemyState.Patrolling:
                movement.PatrolArea();
                CheckForPlayer();
                break;
            case EnemyState.Chasing:
                movement.ChasePlayer();
                CheckForAttack();
                break;
            case EnemyState.Attacking:
                if (!combat.IsCurrentlyAttacking())
                {
                    enemyBase.ChangeState(EnemyState.Chasing);
                }
                break;
            case EnemyState.Maneuvering:
                // Maneuvering logic here.
                break;
            case EnemyState.Hit:
                // Handle Hit logic here.
                break;
        }
    }

    private void CheckForPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer < movement.detectionRadius)
        {
            enemyBase.ChangeState(EnemyState.Chasing);
        }
    }

    private void CheckForAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= combat.meleeRange && combat.canAttack && !combat.IsCurrentlyAttacking())
        {
            enemyBase.ChangeState(EnemyState.Attacking);
            combat.PerformMeleeAttack();
        }
    }

    public void HandleHit()
    {
        movement.enabled = false;
        combat.IsAttacking = false;
        animator.PlayHitAnimation();
        ChangeState(EnemyState.Hit);
    }

    public void HandleDeath()
    {
        movement.enabled = false;
        combat.enabled = false;

        animator.PlayDeathAnimation();
        ChangeState(EnemyState.Dead);
        skeleton.Die();
    }

     public void ChangeState(EnemyState newState)
    {
        if (enemyBase.CurrentState != newState)
        {
            Debug.Log($"State changed from {enemyBase.CurrentState} to {newState}");
            enemyBase.ChangeState(newState);
        }
    }
}
