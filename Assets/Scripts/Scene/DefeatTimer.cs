using UnityEngine;
using TMPro;
using StarterAssets;

public class DefeatTimer : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float timeLimit = 600f; // 10 minutes in seconds
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private TextMeshProUGUI timeText;

    private float timeRemaining;
    private bool defeatTriggered = false;

    private void Start()
    {
        timeRemaining = timeLimit;
    }

    private void Update()
    {
        if (defeatTriggered) return;

        timeRemaining -= Time.deltaTime;

        UpdateText();

        if (timeRemaining <= 0)
        {
            defeatTriggered = true;
            Debug.Log("DEFEAT: time ran out, you didn't reach the helicopter.");
            defeatPanel.SetActive(true);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                ThirdPersonController controller = player.GetComponent<ThirdPersonController>();
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

    private void UpdateText()
    {
        if (timeText == null) return;

        float displayedTime = Mathf.Max(timeRemaining, 0);
        int minutes = Mathf.FloorToInt(displayedTime / 60);
        int seconds = Mathf.FloorToInt(displayedTime % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}