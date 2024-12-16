using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider; // The slider for the health bar
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0); // Offset above the enemy

    private Transform enemyTransform;
    private EnemyBase enemyBase;

    private void Start()
    {
        // Attach to the parent enemy
        enemyTransform = transform.parent;
        enemyBase = enemyTransform.GetComponent<EnemyBase>();

        if (enemyBase == null)
        {
            Debug.LogError("EnemyBase script not found on the parent object.");
            return;
        }

        // Initialize the slider
        healthSlider.maxValue = enemyBase.maxHealth;
        healthSlider.value = enemyBase.CurrentHealth;
    }

    private void Update()
    {
        // Update the health bar's position
        transform.position = enemyTransform.position + offset;

        // Update the slider value to reflect current health
        if (enemyBase != null)
        {
            healthSlider.value = enemyBase.CurrentHealth;
        }
    }
}
