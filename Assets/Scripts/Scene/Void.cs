using CharacterScripts;
using UnityEngine;

public class Void : MonoBehaviour
{
    private bool triggered;

    [SerializeField] private AudioSource sfxFall; // Bloque C - grito de caida (2D, en el jugador)

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;

        triggered = true;
        if (sfxFall != null) sfxFall.Play(); // Bloque C
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
