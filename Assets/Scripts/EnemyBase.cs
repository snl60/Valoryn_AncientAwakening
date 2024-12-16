using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float speed = 2f;
    public int maxHealth = 100;
    private int currentHealth;

    public int CurrentHealth => currentHealth;

    protected EnemyState currentState = EnemyState.Idle;
    public EnemyState CurrentState => currentState;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"Enemy initiated with {currentHealth} Health.");
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState == EnemyState.Dead) return;

        if (currentState != newState)
        {            
            Debug.Log($"State changed from {currentState} to {newState}");
            currentState = newState;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;

        EnemySkeletonStateController stateController = GetComponent<EnemySkeletonStateController>();
        if (stateController != null)
        {
            if (currentHealth <= 0)
            {
                stateController.HandleDeath();
            }
            else
            {
                stateController.HandleHit();
            }
        }
    }

    public virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}
