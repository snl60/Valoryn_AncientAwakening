using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("CombatCollider"),
            true
        );

        Debug.Log("Player and CombatCollider layers will not collide.");
    }
}
