using System.Collections;
using UnityEngine;

// Zona de la oficina (bloque D - Rebeca).
// Al entrar: baja el ambiente de la ciudad y entra el viento interior.
// Al salir: vuelve todo como estaba.
// Usa fades por codigo en lugar de snapshots, porque el mixer es uno solo
// y los snapshots ya los usa la musica adaptativa (bloque B).
[RequireComponent(typeof(Collider))]
public class Office : MonoBehaviour
{
    [Header("Fuentes de ambiente")]
    [SerializeField] private AudioSource exteriorAmbience;
    // El ambiente de la ciudad (city ambience)
    [SerializeField] private AudioSource officeAudioSource;
    // El viento interior de la oficina (cave wind)

    [Header("Volumenes")]
    [SerializeField] private float exteriorVolumeOutside = 0.73f;
    // Volumen de la ciudad cuando estas afuera
    [SerializeField] private float exteriorVolumeInside = 0.25f;
    // Volumen de la ciudad cuando estas adentro: se escucha lejos, no desaparece
    [SerializeField] private float officeVolume = 1f;

    [Header("Transicion")]
    [SerializeField] private float transitionTime = 0.8f;
    [SerializeField] private string playerTag = "Player";

    private Coroutine fade;

    private void Reset()
    {
        // Al agregar el script, el collider se marca como trigger automaticamente
        GetComponent<Collider>().isTrigger = true;
    }

    private void Start()
    {
        // El viento interior arranca apagado
        if (officeAudioSource != null)
        {
            officeAudioSource.volume = 0f;
            officeAudioSource.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (officeAudioSource != null && !officeAudioSource.isPlaying)
        {
            officeAudioSource.Play();
        }

        StartFade(exteriorVolumeInside, officeVolume, false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        StartFade(exteriorVolumeOutside, 0f, true);
    }

    private void StartFade(float exteriorTarget, float officeTarget, bool stopOfficeAtEnd)
    {
        // Si el jugador entra y sale rapido, cortamos el fade anterior
        if (fade != null) StopCoroutine(fade);
        fade = StartCoroutine(Fade(exteriorTarget, officeTarget, stopOfficeAtEnd));
    }

    private IEnumerator Fade(float exteriorTarget, float officeTarget, bool stopOfficeAtEnd)
    {
        float exteriorStart = exteriorAmbience != null ? exteriorAmbience.volume : 0f;
        float officeStart = officeAudioSource != null ? officeAudioSource.volume : 0f;
        float elapsed = 0f;

        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float k = Mathf.Clamp01(elapsed / transitionTime);

            if (exteriorAmbience != null) exteriorAmbience.volume = Mathf.Lerp(exteriorStart, exteriorTarget, k);
            if (officeAudioSource != null) officeAudioSource.volume = Mathf.Lerp(officeStart, officeTarget, k);

            yield return null;
        }

        if (stopOfficeAtEnd && officeAudioSource != null)
        {
            officeAudioSource.Stop();
        }

        fade = null;
    }
}
