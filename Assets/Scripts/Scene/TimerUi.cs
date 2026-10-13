using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [Header("Piezas")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI stateLabel;
    // Opcional: el texto de "QUEDA POCO TIEMPO" / "¡EVACUA YA!"
    [SerializeField] private Image barFill;
    // Opcional: la barra fina. Su Image Type tiene que ser Filled, Horizontal

    [Header("Umbrales (segundos)")]
    [SerializeField] private int warningSeconds = 60;
    [SerializeField] private int criticalSeconds = 15;
    [SerializeField] private int finalSeconds = 5;

    [Header("Colores")]
    [SerializeField] private Color normalColor = new Color32(0xED, 0xE6, 0xDA, 0xFF);
    [SerializeField] private Color warningColor = new Color32(0xF5, 0xA3, 0x00, 0xFF);
    [SerializeField] private Color criticalColor = new Color32(0xE5, 0x48, 0x3D, 0xFF);

    [Header("Textos")]
    [SerializeField] private string warningText = "QUEDA POCO TIEMPO";
    [SerializeField] private string criticalText = "¡EVACUÁ YA!";

    private enum TimerState { Normal, Warning, Critical }

    private TimerState currentState;
    private bool stateInitialized;
    private RectTransform timeRect;

    private void Awake()
    {
        timeRect = timeText.rectTransform;

        if (stateLabel != null)
        {
            stateLabel.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        DefeatTimer.OnTimeChanged += HandleTimeChanged;
        // Nos suscribimos al aviso del timer
    }

    private void OnDisable()
    {
        DefeatTimer.OnTimeChanged -= HandleTimeChanged;
        DOTween.Kill(this);
    }

    private void HandleTimeChanged(int seconds, float fraction)
    {
        TimerState newState = seconds <= criticalSeconds ? TimerState.Critical
                            : seconds <= warningSeconds ? TimerState.Warning
                            : TimerState.Normal;

        if (!stateInitialized || newState != currentState)
        {
            stateInitialized = true;
            currentState = newState;
            ApplyState();
        }

        timeText.text = FormatTime(seconds);

        if (barFill != null)
        {
            barFill.fillAmount = fraction;
        }

        if (currentState == TimerState.Critical)
        {
            Heartbeat(seconds <= finalSeconds);
        }
    }

    private string FormatTime(int seconds)
    {
        int minutes = seconds / 60;
        int secs = seconds % 60;

        string colon = ":";


        return minutes.ToString("00") + colon + secs.ToString("00");
    }

    private void ApplyState()
    {
        DOTween.Kill(this);
        timeRect.localScale = Vector3.one;

        Color color = currentState == TimerState.Critical ? criticalColor
                    : currentState == TimerState.Warning ? warningColor
                    : normalColor;

        timeText.color = color;

        if (barFill != null)
        {
            barFill.color = color;
        }

        if (stateLabel != null)
        {
            bool show = currentState != TimerState.Normal;
            stateLabel.gameObject.SetActive(show);
            stateLabel.text = currentState == TimerState.Critical ? criticalText : warningText;
            stateLabel.color = color;
        }

        if (currentState == TimerState.Warning && barFill != null)
        {
            // En alerta, la barra late suave
            barFill.DOFade(0.5f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetId(this);
        }
    }

    private void Heartbeat(bool strong)
    {
        DOTween.Kill(this);
        timeRect.localScale = Vector3.one;

        float bigBeat = strong ? 1.24f : 1.12f;
        float smallBeat = strong ? 1.12f : 1.06f;

        // "Pum-pum": un golpe fuerte y uno mas suave
        Sequence beat = DOTween.Sequence().SetId(this);
        beat.Append(timeRect.DOScale(bigBeat, 0.08f));
        beat.Append(timeRect.DOScale(1f, 0.1f));
        beat.Append(timeRect.DOScale(smallBeat, 0.08f));
        beat.Append(timeRect.DOScale(1f, 0.2f));

        if (strong && barFill != null)
        {
            // En los ultimos segundos, la barra tambien late
            barFill.color = criticalColor;
            beat.Insert(0f, barFill.DOFade(0.4f, 0.15f));
            beat.Insert(0.15f, barFill.DOFade(1f, 0.25f));
        }
    }
}