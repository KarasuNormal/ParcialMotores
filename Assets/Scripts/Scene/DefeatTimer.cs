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

    private float timeRemaining;
    private int lastSecondNotified = -1;
    private bool stopped = false;

    private void OnEnable()
    {
        GameEvents.OnVictory += StopTimer;
        GameEvents.OnDefeat += StopTimer;
        // Cuando el juego termina, por la razon que sea, el timer se frena solo
    }

    private void OnDisable()
    {
        GameEvents.OnVictory -= StopTimer;
        GameEvents.OnDefeat -= StopTimer;
    }

    private void Start()
    {
        timeRemaining = timeLimit;
    }

    private void Update()
    {
        if (stopped) return;

        timeRemaining -= Time.deltaTime;

        NotifyTime();

        if (timeRemaining <= 0)
        {
            stopped = true;
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
                }
            }

            GameEvents.RaiseDefeat();
            // Avisamos que se perdio por tiempo
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
        stopped = true;
    }
}
