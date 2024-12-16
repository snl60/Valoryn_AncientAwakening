using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public PressurePlate[] pressurePlates; // Assign in the inspector
    public GameObject door; // Door to open upon puzzle completion

    private void Update()
    {
        if (AllPlatesActivated())
        {
            Debug.Log("Puzzle solved!");
            OpenDoor();
        }
    }

    private bool AllPlatesActivated()
    {
        foreach (PressurePlate plate in pressurePlates)
        {
            if (!plate.isActivated) return false;
        }
        return true;
    }

    private void OpenDoor()
    {
        door.SetActive(false); // Disables the door object
    }
}
