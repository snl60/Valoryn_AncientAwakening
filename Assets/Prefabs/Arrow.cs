using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 10f;   // Speed of the arrow
    [SerializeField] private float lifetime = 5f; // Time before the arrow is destroyed
    // [SerializeField] private int damage = 10;    // Damage dealt by the arrow

    private Vector2 direction = Vector2.right;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized; // Normalize to ensure consistent speed
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        Destroy(gameObject, lifetime); // Destroy arrow after lifetime
    }

    private void Update()
    {
        // Move the arrow in the set direction
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    // Check if the arrow hits an enemy
    //    if (collision.CompareTag("Enemy"))
    //    {
    //        Debug.Log($"Arrow hit {collision.name}!");
    //        // Apply damage to the enemy (requires an enemy script)
    //        Enemy enemy = collision.GetComponent<Enemy>();
    //        if (enemy != null)
    //        {
    //            enemy.TakeDamage(damage);
    //        }

    //        // Destroy the arrow on impact
    //        Destroy(gameObject);
    //    }
    //}
}
