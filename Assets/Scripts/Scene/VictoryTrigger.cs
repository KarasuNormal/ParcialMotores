using UnityEngine;
using CharacterScripts;

public class VictoryTrigger : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;
    private DefeatTimer defeatTimer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("VICTORY! The player reached the helicopter.");
            victoryPanel.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (defeatTimer != null)
            {
                defeatTimer.StopTimer();
            }

            CharacterControllerCs controller = other.GetComponent<CharacterControllerCs>();
            if (controller != null)
            {
                controller.enabled = false;
                Debug.Log("Player control DISABLED (Victory).");
            }
            else
            {
                Debug.Log("ThirdPersonController not found to disable.");
            }
        }
    }
}