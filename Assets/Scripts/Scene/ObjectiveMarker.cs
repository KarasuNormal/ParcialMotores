using DG.Tweening;
using UnityEngine;

public class ObjectiveMarker : MonoBehaviour
{
    [Header("Chevrones, de arriba hacia abajo")]
    [SerializeField] private SpriteRenderer[] chevrons;

    [Header("Cascada")]
    [SerializeField] private float cycleDuration = 1.2f;
    // Cuanto dura una vuelta completa de la luz
    [SerializeField] private float delayBetween = 0.15f;
    // El desfase entre un chevron y el siguiente
    [SerializeField] private float minAlpha = 0.15f;
    [SerializeField] private float maxAlpha = 0.9f;
    [SerializeField] private float travel = 0.3f;
    // Cuanto baja cada chevron mientras se ilumina, en unidades del mundo

    [Header("Orientacion")]
    [SerializeField] private bool faceCamera = true;
    // Gira el marcador para que siempre mire a la camara

    private Transform cameraTransform;

    private void Start()
    {
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        for (int i = 0; i < chevrons.Length; i++)
        {
            StartCascade(chevrons[i], i * delayBetween);
        }
    }

    private void StartCascade(SpriteRenderer chevron, float delay)
    {
        Transform t = chevron.transform;
        Vector3 basePosition = t.localPosition;
        Vector3 top = basePosition + Vector3.up * travel * 0.5f;
        Vector3 bottom = basePosition + Vector3.down * travel * 0.5f;

        // Arranca arriba y apagado
        t.localPosition = top;
        Color color = chevron.color;
        color.a = minAlpha;
        chevron.color = color;

        float rise = cycleDuration * 0.3f;
        float fall = cycleDuration * 0.35f;
        float rest = cycleDuration - rise - fall;

        Sequence sequence = DOTween.Sequence().SetId(this);

        // 1. Se ilumina mientras baja a su lugar
        sequence.Append(chevron.DOFade(maxAlpha, rise).SetEase(Ease.OutSine));
        sequence.Join(t.DOLocalMove(basePosition, rise).SetEase(Ease.OutSine));

        // 2. Se apaga mientras sigue bajando
        sequence.Append(chevron.DOFade(minAlpha, fall).SetEase(Ease.InSine));
        sequence.Join(t.DOLocalMove(bottom, fall).SetEase(Ease.InSine));

        // 3. Espera, y vuelve a empezar
        sequence.AppendInterval(rest);
        sequence.SetDelay(delay);
        sequence.SetLoops(-1, LoopType.Restart);
    }

    private void LateUpdate()
    {
        if (!faceCamera || cameraTransform == null) return;

        // Giramos solo en horizontal, asi los chevrones siguen apuntando hacia abajo
        Vector3 direction = transform.position - cameraTransform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnDisable()
    {
        DOTween.Kill(this);
    }
}
