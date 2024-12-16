using UnityEngine;

public class Block : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isMoving = false;
    public float moveSpeed = 2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isMoving) return;

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input != Vector2.zero)
        {
            Vector2 targetPosition = (Vector2)transform.position + input;

            // Check if the target position is valid (no walls or other blocks)
            if (CanMoveTo(targetPosition))
            {
                StartCoroutine(MoveBlock(targetPosition));
            }
        }
    }

    private bool CanMoveTo(Vector2 targetPosition)
    {
        // Raycast to detect collisions with walls or other blocks
        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetPosition - (Vector2)transform.position, 1f);
        return hit.collider == null; // Move only if no collision
    }

    private System.Collections.IEnumerator MoveBlock(Vector2 targetPosition)
    {
        isMoving = true;

        while ((Vector2)transform.position != targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
    }
}
