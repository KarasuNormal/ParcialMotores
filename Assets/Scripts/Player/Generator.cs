
using UnityEngine;
using CharacterScripts;

public class Generator : MonoBehaviour
{
    [SerializeField] private GeneratorManager manager;

    private StarterAssetsInputs playerInput;
    private bool playerNearby;
    private bool isActivated;

    private void Update()
    {
        if (!playerNearby || isActivated || playerInput == null)
            return;

        if (playerInput.interact)
        {
            isActivated = true;
            playerInput.InteractInput(false);

            manager.ActivateGenerator();

            Debug.Log("Generator activated: " + gameObject.name);

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StarterAssetsInputs input = other.GetComponentInParent<StarterAssetsInputs>();

        if (input != null)
        {
            playerInput = input;
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        StarterAssetsInputs input = other.GetComponentInParent<StarterAssetsInputs>();

        if (input != null && input == playerInput)
        {
            playerInput = null;
            playerNearby = false;
        }
    }
}
