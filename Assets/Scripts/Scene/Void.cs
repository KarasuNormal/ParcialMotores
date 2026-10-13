using CharacterScripts;
using UnityEngine;

public class Void : MonoBehaviour
{
    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;

        triggered = true;
        Debug.Log("DEFEAT! The player fell into the void.");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CharacterControllerCs controller = other.GetComponent<CharacterControllerCs>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        GameEvents.RaiseDefeat();
        // Avisamos que se perdio. La UI y el timer reaccionan solos
    }
}
