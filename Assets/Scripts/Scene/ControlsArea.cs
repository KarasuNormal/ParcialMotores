using System;
using UnityEngine;

public class ControlsArea : MonoBehaviour
{
    public static event Action<TutorialType> OnTutorialEntered;
    public static event Action<TutorialType> OnTutorialExited;
    // Los "avisos" que manda la zona. No sabe quien los escucha:
    // la UI se suscribe y reacciona sola (patron Observer)

    [SerializeField] private TutorialType tutorialType;
    // Que tutorial corresponde a esta zona (se elige en el Inspector)

    private bool tutorialShown;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !tutorialShown)
        {
            tutorialShown = true;
            OnTutorialEntered?.Invoke(tutorialType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnTutorialExited?.Invoke(tutorialType);
        }
    }
}