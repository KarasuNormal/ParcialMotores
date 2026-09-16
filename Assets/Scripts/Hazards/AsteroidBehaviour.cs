using CharacterScripts;
using UnityEngine;

public class AsteroidBehaviour : MonoBehaviour
{
    private GameObject warningMarker;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (warningMarker != null)
        {
            Destroy(warningMarker);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("DEFEAT! Asteroid killed you.");

            DefeatTimer timer = FindFirstObjectByType<DefeatTimer>();
            if (timer != null)
            {
                timer.Defeat();
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            CharacterControllerCs controller = collision.gameObject.GetComponent<CharacterControllerCs>();
            if (controller != null)
            {
                controller.enabled = false;
            }
        }

        Destroy(gameObject);
    }
}