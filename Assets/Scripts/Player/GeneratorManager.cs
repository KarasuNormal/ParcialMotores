

using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    [Header("Generators")]
    [SerializeField] private int requiredGenerators = 3;

    [Header("Door")]
    [SerializeField] private GameObject door;

    [Header("Audio - Bloque C")]
    [SerializeField] private AudioSource sfxDoor;
    // Va en un objeto propio ubicado en la puerta (no hijo): Destroy(door) cortaria el sonido

    private int activatedGenerators;
    private bool doorOpened;

    public void ActivateGenerator()
    {
        if (doorOpened)
            return;

        activatedGenerators++;

        GameEvents.RaiseGeneratorActivated(activatedGenerators, requiredGenerators); // Bloque E: voces

        Debug.Log("Generators activated: " +
                  activatedGenerators + "/" + requiredGenerators);

        if (activatedGenerators >= requiredGenerators)
            OpenDoor();
    }

    private void OpenDoor()
    {
        doorOpened = true;

        GameEvents.RaiseDoorOpened(); // Bloque E: voces

        Debug.Log("All generators activated. Destroying door.");

        if (sfxDoor != null) sfxDoor.Play(); // Bloque C

        Destroy(door);
    }
}
