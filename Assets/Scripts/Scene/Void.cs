using StarterAssets;
using UnityEngine;

public class Void : MonoBehaviour
{
    [SerializeField] private GameObject defeatPanel;
   private DefeatTimer defeatTimer;

    private void Start()
    {
        defeatTimer = FindFirstObjectByType<DefeatTimer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            defeatTimer.StopTimer();
            Debug.Log("DEFEAT! The player fell into the void.");
            defeatPanel.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            ThirdPersonController controller = other.GetComponent<ThirdPersonController>();
            if (controller != null)
            {
                controller.enabled = false;
                
            }
        }
    }
}
