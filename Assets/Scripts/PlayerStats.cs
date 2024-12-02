using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100;
    private float currentHealth;
    
    [SerializeField] private float maxStamina = 100f;
    private float currentStamina;
    
    [SerializeField] private int maxArrowCount = 20;
    private int currentArrowCount;
    
    [SerializeField] private int level = 1;
    private int experiencePoints = 0;
    
    [SerializeField] private string equippedMeleeWeapon = "Simple Sword";
    [SerializeField] private string equippedRangedWeapon = "Basic Bow";

    private PlayerHUD playerHUD;

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentArrowCount = maxArrowCount;

        playerHUD = GetComponent<PlayerHUD>();

        if (playerHUD != null)
        {
            playerHUD.UpdateHealth(currentHealth);
            playerHUD.UpdateStamina(currentStamina);
            playerHUD.UpdateArrowCount(currentArrowCount);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        if (playerHUD != null)
        {
            playerHUD.UpdateHealth(currentHealth);
        }
    }

    public void UseStamina(float amount)
    {
        currentStamina = Mathf.Max(0, currentStamina - amount);
        if (playerHUD != null)
        {
            playerHUD.UpdateStamina(currentStamina);
        }
    }

    public void TrackArrowCount()
    {
        if (currentArrowCount > 0)
        {
            currentArrowCount--;
            if (playerHUD != null)
            {
                playerHUD.UpdateArrowCount(currentArrowCount);
            }
        }
    }

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public float MaxStamina => maxStamina;
    public float CurrentStamina => currentStamina;

    public int MaxArrowCount => maxArrowCount;
    public int CurrentArrowCount => currentArrowCount;
}