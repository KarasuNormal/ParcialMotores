using UnityEngine;

public class AsteroidBehaviour : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 3f;

    private bool hasLanded = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded)
        {
            hasLanded = true;
            Debug.Log("Asteroid landed, will be destroyed in " + destroyDelay + " seconds.");

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            Destroy(gameObject, destroyDelay);
        }
    }
}