

using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    [Header("Generators")]
    [SerializeField] private int requiredGenerators = 3;

    [Header("Door")]
    [SerializeField] private GameObject door;

    private int activatedGenerators;
    private bool doorOpened;

    public void ActivateGenerator()
    {
        if (doorOpened)
            return;

        activatedGenerators++;

        Debug.Log("Generators activated: " +
                  activatedGenerators + "/" + requiredGenerators);

        if (activatedGenerators >= requiredGenerators)
            OpenDoor();
    }

    private void OpenDoor()
    {
        doorOpened = true;

        Debug.Log("All generators activated. Destroying door.");

        Destroy(door);
    }
}
