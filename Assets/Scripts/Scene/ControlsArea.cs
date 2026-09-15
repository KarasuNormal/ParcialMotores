using UnityEngine;

public class ControlsArea : MonoBehaviour
{
    [SerializeField] private GameObject controlsPanel;

    private bool tutorialShown;

    private void Start()
    {
        controlsPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !tutorialShown)
        {
            controlsPanel.SetActive(true);
            tutorialShown = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controlsPanel.SetActive(false);
        }
    }
}