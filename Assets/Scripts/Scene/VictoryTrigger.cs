using UnityEngine;
using CharacterScripts;

public class VictoryTrigger : MonoBehaviour
{
    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;

        triggered = true;
        Debug.Log("VICTORY! The player reached the helicopter.");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CharacterControllerCs controller = other.GetComponent<CharacterControllerCs>();
        if (controller != null)
        {
            controller.enabled = false;
            Debug.Log("Player control DISABLED (Victory).");
        }

        GameEvents.RaiseVictory();
        // Avisamos que se gano. La UI y el timer reaccionan solos
    }
}
