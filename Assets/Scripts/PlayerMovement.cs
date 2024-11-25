using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Movement speed

    // Animation clips for each movement and idle direction
    [SerializeField] private AnimationClip idleUpClip;
    [SerializeField] private AnimationClip idleDownClip;
    [SerializeField] private AnimationClip idleRightClip;
    [SerializeField] private AnimationClip moveUpClip;
    [SerializeField] private AnimationClip moveDownClip;
    [SerializeField] private AnimationClip moveRightClip;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerAttack playerAttack;
    private Vector2 moveInput;
    private Vector2 lastDirection; // Tracks the last direction the player moved

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAttack = GetComponent<PlayerAttack>();

        if (rb == null)
            Debug.LogError("Rigidbody2D is missing!");
        if (animator == null)
            Debug.LogError("Animator is missing!");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // Update last direction if a cardinal direction is pressed
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                lastDirection = new Vector2(moveInput.x > 0 ? 1 : -1, 0); // Horizontal (Left/Right)
            }
            else
            {
                lastDirection = new Vector2(0, moveInput.y > 0 ? 1 : -1); // Vertical (Up/Down)
            }
        }
    }

    private void Update()
    {
        PlayAnimation();
    }

    private void FixedUpdate()
    {
        // Normalize diagonal movement to ensure consistent speed
        rb.velocity = moveInput.normalized * moveSpeed;
    }

    private void PlayAnimation()
    {
        if (playerAttack.isAttacking) { return; }
        
        // Determine if the player is idle or moving
        bool isMoving = moveInput != Vector2.zero;

        // Play movement animations
        if (isMoving)
        {
            if (moveInput.y > 0)
                animator.Play(moveUpClip.name);
            else if (moveInput.y < 0)
                animator.Play(moveDownClip.name);
            else if (moveInput.x > 0)
                animator.Play(moveRightClip.name);
            else if (moveInput.x < 0)
                animator.Play(moveRightClip.name); // Use right animation and flip it
        }
        // Play idle animations based on last direction
        else
        {
            if (lastDirection.y > 0)
                animator.Play(idleUpClip.name);
            else if (lastDirection.y < 0)
                animator.Play(idleDownClip.name);
            else if (lastDirection.x > 0)
                animator.Play(idleRightClip.name);
            else
                animator.Play(idleRightClip.name); // Use right idle animation and flip it
        }

        // Flip the sprite for left-facing animations
        spriteRenderer.flipX = lastDirection.x < 0;
    }

    public Vector2 GetLastDirection() {  return lastDirection; }
}
