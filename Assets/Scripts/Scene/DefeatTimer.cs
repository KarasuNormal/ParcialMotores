using System;
using UnityEngine;
using CharacterScripts;

public class DefeatTimer : MonoBehaviour
{
    public static event Action<int, float> OnTimeChanged;
    // El "aviso" del timer: (segundos que quedan, fraccion de tiempo que queda de 0 a 1).
    // El timer no sabe quien lo escucha: la UI se suscribe y se actualiza sola

    [Header("Configuration")]
    [SerializeField] private float timeLimit = 600f;
    [SerializeField] private GameObject defeatPanel;

    private float timeRemaining;
    private int lastSecondNotified = -1;
    private bool defeatTriggered = false;

    private void Start()
    {
        timeRemaining = timeLimit;
    }

    private void Update()
    {
        if (defeatTriggered) return;

        timeRemaining -= Time.deltaTime;

        NotifyTime();

        if (timeRemaining <= 0)
        {
            Defeat();
            Debug.Log("DEFEAT: time ran out, you didn't reach the helicopter.");

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                CharacterControllerCs controller = player.GetComponent<CharacterControllerCs>();
                if (controller != null)
                {
                    controller.enabled = false;
                    Debug.Log("Player control DISABLED (Defeat).");
                }
                else
                {
                    Debug.Log("ThirdPersonController not found to disable.");
                }
            }
        }
    }

    private void NotifyTime()
    {
        int seconds = Mathf.FloorToInt(Mathf.Max(timeRemaining, 0f));

        if (seconds == lastSecondNotified) return;
        // Solo avisamos cuando cambia el segundo, no en cada frame

        lastSecondNotified = seconds;
        OnTimeChanged?.Invoke(seconds, Mathf.Clamp01(timeRemaining / timeLimit));
    }

    public void StopTimer()
    {
        defeatTriggered = true;
    }

    public void Defeat()
    {
        defeatTriggered = true;
        defeatPanel.SetActive(true);
    }
}