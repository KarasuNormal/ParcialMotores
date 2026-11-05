using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFeedbackUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image damageOverlay;
    [SerializeField] private Image lowHealthOverlay;

    [Header("Daño")]
    [SerializeField] private float damageAlpha = 0.7f;
    [SerializeField] private float fadeDuration = 0.4f;

    [Header("Poca vida")]
    [SerializeField] private float lowHealthAlpha = 0.3f;

    private Coroutine damageCoroutine;

    private void Awake()
    {
        SetImageAlpha(damageOverlay, 0f);
        SetImageAlpha(lowHealthOverlay, 0f);
    }

    public void ShowDamage()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }

        damageCoroutine = StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        Color color = damageOverlay.color;

        color.a = damageAlpha;
        damageOverlay.color = color;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            color.a = Mathf.Lerp(
                damageAlpha,
                0f,
                elapsed / fadeDuration
            );

            damageOverlay.color = color;

            yield return null;
        }

        color.a = 0f;
        damageOverlay.color = color;

        damageCoroutine = null;
    }

    public void SetLowHealth(bool isLowHealth)
    {
        SetImageAlpha(
            lowHealthOverlay,
            isLowHealth ? lowHealthAlpha : 0f
        );
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        if (image == null)
            return;

        Color color = image.color;

        color.a = alpha;

        image.color = color;
    }
}