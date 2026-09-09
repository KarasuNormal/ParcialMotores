using StarterAssets;
using UnityEngine;

public class Void : MonoBehaviour
{
    [SerializeField] private GameObject defeatPanel;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
