using UnityEngine;
using DG.Tweening;

// Cartel de INTERACTUAR [E] en el HUD.
// Escucha a GameEvents y aparece / desaparece con animacion.
// No sabe nada de los generadores: solo reacciona a los avisos.
//
// IMPORTANTE: este objeto tiene que quedar ACTIVO en la escena.
// Se oculta con transparencia (CanvasGroup), no desactivandolo,
// porque si esta desactivado no puede escuchar los eventos.
[RequireComponent(typeof(CanvasGroup))]
public class InteractPromptUI : MonoBehaviour
{
    [Header("Animacion")]
    [SerializeField] private float fadeDuration = 0.2f;

    [Tooltip("Tamanio desde el que 'crece' al aparecer")]
    [SerializeField] private float startScale = 0.85f;

    [Tooltip("Cuanto late mientras esta visible (1 = nada)")]
    [SerializeField] private float pulseScale = 1.05f;
    [SerializeField] private float pulseDuration = 0.6f;

    private CanvasGroup canvasGroup;

    // Dos animaciones separadas: la de entrar/salir y el latido.
    // DOTween no permite loops infinitos adentro de una secuencia,
    // por eso el latido arranca aparte cuando termina la entrada.
    private Tween fadeAnim;
    private Tween pulseAnim;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        // Arranca invisible
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * startScale;
    }

    // Nos suscribimos a los avisos
    private void OnEnable()
    {
        GameEvents.OnInteractPromptShow += Show;
        GameEvents.OnInteractPromptHide += Hide;
        GameEvents.OnVictory += Hide;
        GameEvents.OnDefeat += Hide;
    }

    // Y nos desuscribimos para no dejar referencias colgadas
    private void OnDisable()
    {
        GameEvents.OnInteractPromptShow -= Show;
        GameEvents.OnInteractPromptHide -= Hide;
        GameEvents.OnVictory -= Hide;
        GameEvents.OnDefeat -= Hide;

        StopAnims();
    }

    private void Show()
    {
        StopAnims();

        // Aparece y crece a su tamanio normal...
        fadeAnim = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1f, fadeDuration))
            .Join(transform.DOScale(1f, fadeDuration).SetEase(Ease.OutBack))
            // ...y cuando termina, empieza a latir suave
            .OnComplete(StartPulse);
    }

    private void StartPulse()
    {
        pulseAnim = transform.DOScale(pulseScale, pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void Hide()
    {
        StopAnims();

        fadeAnim = DOTween.Sequence()
            .Append(canvasGroup.DOFade(0f, fadeDuration))
            .Join(transform.DOScale(startScale, fadeDuration).SetEase(Ease.InBack));
    }

    // Corta cualquier animacion que este corriendo antes de empezar otra
    private void StopAnims()
    {
        fadeAnim?.Kill();
        pulseAnim?.Kill();
    }
}