using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// Controla los estados de la musica adaptativa (bloque B).
// Las tres canciones suenan siempre en loop; cada estado es un snapshot
// del mixer que decide cual se escucha. La musica solo avanza, nunca vuelve atras.
public class MusicStateController : MonoBehaviour
{
    public enum MusicState { Exploracion = 0, Tension = 1, Rescate = 2, Final = 3 }

    public static MusicStateController Instance { get; private set; }

    [Header("Snapshots del mixer")]
    [SerializeField] private AudioMixerSnapshot exploracion;
    [SerializeField] private AudioMixerSnapshot tension;
    [SerializeField] private AudioMixerSnapshot rescate;
    [SerializeField] private AudioMixerSnapshot final;

    [Header("Tiempos de transicion (segundos)")]
    [SerializeField] private float fadeEntrada = 2f;
    // Fade progresivo al empezar el nivel
    [SerializeField] private float fadeTension = 2f;
    // Fade progresivo para que el peligro crezca de a poco
    [SerializeField] private float fadeRescate = 0.5f;
    // Fade corto para marcar el tramo final
    [SerializeField] private float fadeFinal = 0.1f;
    // Casi corte: la musica se apaga y entra el stinger

    [Header("Stingers")]
    [SerializeField] private AudioSource stingerSource;
    // Este AudioSource tiene que salir por el grupo Musica (no por un subgrupo)
    [SerializeField] private AudioClip stingerVictoria;
    [SerializeField] private AudioClip stingerDerrota;

    private MusicState current = MusicState.Exploracion;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private IEnumerator Start()
    {
        // Arranca en silencio y entra la musica de exploracion con un fade
        final.TransitionTo(0f);
        yield return null;
        exploracion.TransitionTo(fadeEntrada);
    }

    // Lo llaman los triggers de zona
    public void GoToState(MusicState next)
    {
        // Solo avanza: si ya estamos en ese estado o en uno posterior, no hace nada
        if (next <= current) return;

        current = next;

        switch (next)
        {
            case MusicState.Tension:
                tension.TransitionTo(fadeTension);
                break;
            case MusicState.Rescate:
                rescate.TransitionTo(fadeRescate);
                break;
        }
    }

    // Llamar desde el evento de victoria
    public void PlayVictory()
    {
        PlayStinger(stingerVictoria);
    }

    // Llamar desde el evento de derrota (tiempo, Void o meteorito)
    public void PlayDefeat()
    {
        PlayStinger(stingerDerrota);
    }

    private void PlayStinger(AudioClip clip)
    {
        // Evita que suenen dos stingers si se disparan dos eventos juntos
        if (current == MusicState.Final) return;

        current = MusicState.Final;
        final.TransitionTo(fadeFinal);

        if (stingerSource != null && clip != null)
        {
            stingerSource.PlayOneShot(clip);
        }
    }
}
