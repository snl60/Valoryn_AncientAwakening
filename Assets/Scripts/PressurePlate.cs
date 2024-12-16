using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool isActivated = false; // Tracks whether the plate is activated
    public Sprite activeSprite;      // Sprite for the "activated" state
    public Sprite inactiveSprite;    // Sprite for the "inactive" state
    public GameObject chestPrefab;   // Prefab to spawn
    public Transform chestSpawnPoint; // Location where the chest will appear

    private SpriteRenderer spriteRenderer;

    private bool hasChestSpawned = false;

    private void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set the initial sprite to the inactive state
        spriteRenderer.sprite = inactiveSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if a "Block" enters the trigger zone
        if (other.CompareTag("Block"))
        {
            if (!isActivated) // Prevent spawning multiple chests
            {
                isActivated = true;
                spriteRenderer.sprite = activeSprite; // Change to active sprite
                SpawnChest(); // Call chest spawn method
                Debug.Log("Pressure plate activated!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if a "Block" exits the trigger zone
        if (other.CompareTag("Block"))
        {
            isActivated = false;
            spriteRenderer.sprite = inactiveSprite; // Change to inactive sprite
            Debug.Log("Pressure plate deactivated!");
        }
    }

    private void SpawnChest()
    {
        if (hasChestSpawned) return;
        
        if (chestPrefab != null && chestSpawnPoint != null)
        {
            Instantiate(chestPrefab, chestSpawnPoint.position, Quaternion.identity);
            hasChestSpawned = true;
            Debug.Log("Chest spawned!");
        }
        else
        {
            Debug.LogWarning("Chest prefab or spawn point not assigned!");
        }
    }
}
