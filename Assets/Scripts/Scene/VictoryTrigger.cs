using UnityEngine;
using StarterAssets;

public class VictoryTrigger : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private DefeatTimer defeatTimer;

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

            ThirdPersonController controller = other.GetComponent<ThirdPersonController>();
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