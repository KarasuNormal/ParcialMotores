using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCard : MonoBehaviour
{
    [Header("Que tutorial muestra esta tarjeta")]
    [SerializeField] private TutorialType tutorialType;

    [Header("Piezas de la tarjeta")]
    [SerializeField] private CanvasGroup cardGroup;
    [SerializeField] private RectTransform stripe;
    [SerializeField] private Image wave;
    [SerializeField] private CanvasGroup label;
    [SerializeField] private CanvasGroup[] texts;
    [SerializeField] private RectTransform[] keys;
    [SerializeField] private Sprite[] keysOn;
    // Opcional: las teclas encendidas, en el mismo orden que "keys"
    [SerializeField] private RectTransform stamp;
    // Opcional: el sello de HECHO

    [Header("Animacion")]
    [SerializeField] private float showDelay = 2f;
    // Segundos que espera la tarjeta antes de aparecer
    [SerializeField] private bool enterFromRight = true;
    // Tildado: entra desde la derecha. Destildado: desde la izquierda
    [SerializeField] private float entryDistance = 120f;
    [SerializeField] private float waitBeforeExit = 1.2f;

    private RectTransform card;
    private Vector2 basePosition;
    private bool isShowing;

    private void Awake()
    {
        card = (RectTransform)transform;
        basePosition = card.anchoredPosition;
        cardGroup.alpha = 0f;
        // La tarjeta arranca invisible pero ACTIVA, asi puede escuchar los avisos

        if (stamp != null)
        {
            stamp.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        ControlsArea.OnTutorialEntered += HandleEntered;
        ControlsArea.OnTutorialExited += HandleExited;
        // Nos suscribimos a los avisos de las zonas de tutorial
    }

    private void OnDisable()
    {
        ControlsArea.OnTutorialEntered -= HandleEntered;
        ControlsArea.OnTutorialExited -= HandleExited;
        DOTween.Kill(this);
    }

    private void HandleEntered(TutorialType type)
    {
        if (type == tutorialType)
        {
            Show();
        }
    }

    private void HandleExited(TutorialType type)
    {
        if (type == tutorialType && isShowing)
        {
            Complete();
        }
    }

    private void Show()
    {
        DOTween.Kill(this);
        isShowing = true;
        cardGroup.alpha = 0f;
        Vector2 entryOffset = (enterFromRight ? Vector2.right : Vector2.left) * entryDistance;
        card.anchoredPosition = basePosition + entryOffset;

        Sequence sequence = DOTween.Sequence().SetId(this);

        // 0. Esperamos unos segundos antes de mostrar la tarjeta
        sequence.AppendInterval(showDelay);
        float start = showDelay;
        // "start" es el momento en que arranca la animacion, despues de la espera

        // 1. La tarjeta entra deslizandose y aparece
        sequence.Append(card.DOAnchorPos(basePosition, 0.35f).SetEase(Ease.OutCubic));
        sequence.Join(cardGroup.DOFade(1f, 0.15f));

        // 2. La franja de peligro crece de abajo hacia arriba
        if (stripe != null)
        {
            stripe.localScale = new Vector3(1f, 0f, 1f);
            sequence.Join(stripe.DOScaleY(1f, 0.2f));
        }

        // 3. Temblor, como una replica
        sequence.Append(card.DOShakeAnchorPos(0.25f, 6f, 30));

        // 4. "INSTRUCCION" parpadea como un cartel electrico fallando
        if (label != null)
        {
            label.alpha = 0f;
            sequence.Insert(start + 0.3f, LabelFlicker());
        }

        // 5. Los textos aparecen
        foreach (CanvasGroup text in texts)
        {
            text.alpha = 0f;
            sequence.Insert(start + 0.4f, text.DOFade(1f, 0.3f));
        }

        // 6. Las teclas caen una por una
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].localScale = Vector3.zero;
            sequence.Insert(start + 0.35f + i * 0.08f, keys[i].DOScale(1f, 0.3f).SetEase(Ease.OutBack));
        }

        sequence.OnComplete(StartWaiting);
    }

    private Sequence LabelFlicker()
    {
        Sequence flicker = DOTween.Sequence();
        flicker.Append(label.DOFade(1f, 0.05f));
        flicker.Append(label.DOFade(0.2f, 0.05f));
        flicker.Append(label.DOFade(1f, 0.05f));
        flicker.Append(label.DOFade(0.3f, 0.05f));
        flicker.Append(label.DOFade(1f, 0.1f));
        return flicker;
    }

    private void StartWaiting()
    {
        // Las teclas "laten" mientras esperamos que el jugador avance
        foreach (RectTransform key in keys)
        {
            key.DOScale(1.06f, 0.6f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetId(this);
        }

        // La onda sismica se dibuja en loop
        if (wave != null)
        {
            wave.type = Image.Type.Filled;
            wave.fillMethod = Image.FillMethod.Horizontal;
            wave.fillAmount = 0f;
            wave.DOFillAmount(1f, 1.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart).SetId(this);
        }
    }

    private void Complete()
    {
        isShowing = false;
        DOTween.Kill(this);

        if (wave != null)
        {
            wave.fillAmount = 1f;
        }

        Sequence sequence = DOTween.Sequence().SetId(this);

        // 1. Las teclas se "aprietan" y se encienden
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].localScale = Vector3.one;

            if (keysOn != null && i < keysOn.Length && keysOn[i] != null)
            {
                keys[i].GetComponent<Image>().sprite = keysOn[i];
            }

            sequence.Join(keys[i].DOPunchScale(Vector3.one * -0.1f, 0.2f, 1, 0f));
        }

        // 2. Cae el sello de HECHO, como un sello de goma
        if (stamp != null)
        {
            stamp.gameObject.SetActive(true);
            stamp.localScale = Vector3.one * 2f;
            stamp.localEulerAngles = new Vector3(0f, 0f, -8f);
            sequence.Join(stamp.DOScale(1f, 0.35f).SetEase(Ease.OutBack));
        }

        // 3. Temblor, espera, y la tarjeta se va hacia la izquierda
        sequence.Append(card.DOShakeAnchorPos(0.2f, 5f, 30));
        sequence.AppendInterval(waitBeforeExit);
        sequence.Append(card.DOAnchorPos(basePosition + Vector2.left * entryDistance, 0.35f).SetEase(Ease.InCubic));
        sequence.Join(cardGroup.DOFade(0f, 0.35f));
    }
}