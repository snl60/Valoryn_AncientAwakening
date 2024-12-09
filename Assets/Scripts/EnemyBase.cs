using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float speed = 2f;
    public int health = 100;

    protected EnemyState currentState = EnemyState.Idle;
    public EnemyState CurrentState => currentState;

    public void ChangeState(EnemyState newState)
    {
        if (currentState != newState)
        {
            Debug.Log($"State changed from {currentState} to (newState)");
            currentState = newState;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}
