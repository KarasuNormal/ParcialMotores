
using UnityEngine;
using CharacterScripts;

public class Generator : MonoBehaviour
{
    [SerializeField] private GeneratorManager manager;
    [SerializeField] private AudioSource sfxActivate; // Bloque C

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
            if (sfxActivate != null) sfxActivate.Play(); // Bloque C

            Debug.Log("Generator activated: " + gameObject.name);

        
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
