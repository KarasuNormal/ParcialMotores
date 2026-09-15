using CharacterScripts;
using UnityEngine;

public class AsteroidBehaviour : MonoBehaviour
{
    private GameObject warningMarker;

    [SerializeField] private GameObject defeatPanel;
    private DefeatTimer defeatTimer;

    public void Initialize(Vector3 targetPosition, float speed, GameObject marker)
    {
        warningMarker = marker;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;

            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (warningMarker != null)
        {
            Destroy(warningMarker);
        }

        if (other.gameObject.CompareTag("Player"))
        {
            defeatTimer.StopTimer();
            Debug.Log("DEFEAT! Asteroid kill you.");
            defeatPanel.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            CharacterControllerCs controller = other.GetComponent<CharacterControllerCs>();
            if (controller != null) {
                controller.enabled = false;
            }
        }

        Destroy(gameObject);
    }
}