using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private TextMeshProUGUI arrowCountText;

    [SerializeField] private PlayerStats playerStats;

    private void Start()
    {
        healthSlider.maxValue = playerStats.MaxHealth;
        healthSlider.value = playerStats.CurrentHealth;

        staminaSlider.maxValue = playerStats.MaxStamina;
        staminaSlider.value = playerStats.CurrentStamina;

        arrowCountText.text = playerStats.CurrentArrowCount.ToString();
    }

    private void Update()
    {
        UpdateHealth(playerStats.CurrentHealth);
        UpdateStamina(playerStats.CurrentStamina);
        UpdateArrowCount(playerStats.CurrentArrowCount);
    }

    public void UpdateHealth(float currentHealth)
    {
        healthSlider.value = currentHealth;
    }

    public void UpdateStamina(float currentStamina)
    {
        staminaSlider.value = currentStamina;
    }

    public void UpdateArrowCount(int currentArrowCount)
    {
        arrowCountText.text = currentArrowCount.ToString();
    }
}
