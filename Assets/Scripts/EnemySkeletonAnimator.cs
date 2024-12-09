using UnityEngine;

public class EnemySkeletonAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private EnemySkeletonMovement movement;
    [SerializeField] private EnemySkeletonCombat combat;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private EnemyBase enemyBase;

    private void Start()
    {
        enemyBase = GetComponent<EnemyBase>();

        if (animator == null)
        {
            Debug.LogError("Animator not assigned in EnemySkeletonAnimator.");
        }
    }

    private void Update()
    {
        switch (enemyBase.CurrentState)
        {
            case EnemyState.Idle:
                PlayIdleAnimation();
                break;
            case EnemyState.Patrolling:
                movement.PatrolArea();
                break;
            case EnemyState.Chasing:
                PlayWalkingAnimation();
                break;
            case EnemyState.Attacking:
                PlayAttackAnimation();
                break;
            case EnemyState.Hit:
                PlayHitAnimation();
                break;
            case EnemyState.Stunned:
                PlayStunnedAnimation();
                break;
            case EnemyState.Dead:
                PlayDeathAnimation();
                break;
        }
    }

    public void PlayIdleAnimation()
    {
        switch (movement.CurrentDirection)
        {
            case "up":
                animator.Play("skeleton_idle_up");
                break;
            case "down":
                animator.Play("skeleton_idle_down");
                break;
            case "right":
                spriteRenderer.flipX = false;
                animator.Play("skeleton_idle_right");
                break;
            case "left":
                spriteRenderer.flipX = true;
                animator.Play("skeleton_idle_right");
                break;
        }
    }

    public void PlayWalkingAnimation()
    {
        switch (movement.CurrentDirection)
        {
            case "up":
                animator.Play("skeleton_walk_up");
                break;
            case "down":
                animator.Play("skeleton_walk_down");
                break;
            case "right":
                spriteRenderer.flipX = false;
                animator.Play("skeleton_walk_right");
                break;
            case "left":
                spriteRenderer.flipX = true;
                animator.Play("skeleton_walk_right");
                break;
        }
    }

    public void PlayAttackAnimation()
    {
        if (combat.canAttack)
        {
            switch (movement.CurrentDirection)
            {
                case "up":
                    animator.Play("skeleton_melee_up");
                    break;
                case "down":
                    animator.Play("skeleton_melee_down");
                    break;
                case "right":
                    spriteRenderer.flipX = false;
                    animator.Play("skeleton_melee_right");
                    break;
                case "left":
                    spriteRenderer.flipX = true;
                    animator.Play("skeleton_melee_right");
                    break;
            }
        }
    }

    public void PlayStunnedAnimation()
    {
        // Stunned animation logic here
    }

    public void PlayHitAnimation()
    {
        switch (movement.CurrentDirection)
        {
            case "up":
                animator.Play("skeleton_hit_up");
                break;
            case "down":
                animator.Play("skeleton_hit_down");
                break;
            case "right":
                spriteRenderer.flipX = false;
                animator.Play("skeleton_hit_right");
                break;
            case "left":
                spriteRenderer.flipX= true;
                animator.Play("skeleton_hit_right");
                break;
        }
    }

    public void PlayDeathAnimation()
    {
        movement.enabled = false;
        combat.enabled = false;
        animator.Play("skeleton_death_right");
    }
}
