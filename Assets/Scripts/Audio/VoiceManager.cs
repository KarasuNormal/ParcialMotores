using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Voces del juego (bloque E).
// Reproduce cada linea una sola vez, en el momento justo, y sin pisar una con otra:
// si llega una linea nueva mientras suena otra, espera su turno.
// Las voces salen por el grupo Voz del mixer, asi el ducking baja la musica solo.
public class VoiceManager : MonoBehaviour
{
    public enum VoiceLine
    {
        Inicio,           // Arranca el nivel
        Meteoritos,       // Avanza un poco y empiezan a caer meteoritos (zona)
        PuertaCerrada,    // Llega frente a la puerta cerrada (zona)
        PrimerGenerador,  // Activa el primer generador
        PuertaAbierta,    // Se abre la puerta con el tercer generador
        SesentaSegundos   // Quedan 60 segundos en el timer
    }

    public static VoiceManager Instance { get; private set; }

    [Header("Fuente (Output: Voz)")]
    [SerializeField] private AudioSource voiceSource;

    [Header("Lineas de voz")]
    [SerializeField] private AudioClip inicio;
    [SerializeField] private AudioClip meteoritos;
    [SerializeField] private AudioClip puertaCerrada;
    [SerializeField] private AudioClip primerGenerador;
    [SerializeField] private AudioClip puertaAbierta;
    [SerializeField] private AudioClip sesentaSegundos;

    [Header("Tiempos")]
    [SerializeField] private float delayInicio = 1.5f;
    // Espera un poco al arrancar, para que entre despues del fade de la musica
    [SerializeField] private float pausaEntreLineas = 0.3f;
    [SerializeField] private int segundosAviso = 60;

    private readonly HashSet<VoiceLine> played = new HashSet<VoiceLine>();
    private readonly Queue<AudioClip> queue = new Queue<AudioClip>();
    private Coroutine playing;
    private bool gameOver;
    private bool doorOpened;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void OnEnable()
    {
        GameEvents.OnGeneratorActivated += HandleGeneratorActivated;
        GameEvents.OnDoorOpened += HandleDoorOpened;
        GameEvents.OnVictory += HandleGameOver;
        GameEvents.OnDefeat += HandleGameOver;
        DefeatTimer.OnTimeChanged += HandleTimeChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnGeneratorActivated -= HandleGeneratorActivated;
        GameEvents.OnDoorOpened -= HandleDoorOpened;
        GameEvents.OnVictory -= HandleGameOver;
        GameEvents.OnDefeat -= HandleGameOver;
        DefeatTimer.OnTimeChanged -= HandleTimeChanged;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(delayInicio);
        Play(VoiceLine.Inicio);
    }

    // La llaman los eventos de abajo y las zonas (VoiceZoneTrigger)
    public void Play(VoiceLine line)
    {
        // Cada linea suena una sola vez, y nada suena despues de ganar o perder
        if (gameOver || played.Contains(line)) return;

        // Si la puerta ya se abrio, no tiene sentido avisar que esta cerrada
        if (line == VoiceLine.PuertaCerrada && doorOpened) return;

        AudioClip clip = GetClip(line);
        if (clip == null || voiceSource == null) return;

        played.Add(line);
        queue.Enqueue(clip);

        if (playing == null)
        {
            playing = StartCoroutine(PlayQueue());
        }
    }

    private IEnumerator PlayQueue()
    {
        while (queue.Count > 0 && !gameOver)
        {
            AudioClip clip = queue.Dequeue();
            voiceSource.clip = clip;
            voiceSource.Play();

            // Esperamos a que termine la linea antes de pasar a la siguiente
            yield return new WaitForSecondsRealtime(clip.length + pausaEntreLineas);
        }

        playing = null;
    }

    private void HandleGeneratorActivated(int activated, int required)
    {
        if (activated == 1)
        {
            Play(VoiceLine.PrimerGenerador);
        }
    }

    private void HandleDoorOpened()
    {
        doorOpened = true;
        Play(VoiceLine.PuertaAbierta);
    }

    private void HandleTimeChanged(int seconds, float fraction)
    {
        if (seconds <= segundosAviso && seconds > 0)
        {
            Play(VoiceLine.SesentaSegundos);
        }
    }

    private void HandleGameOver()
    {
        // Al ganar o perder se cortan las voces y se descarta lo que estaba en espera
        gameOver = true;
        queue.Clear();
        if (voiceSource != null) voiceSource.Stop();
    }

    private AudioClip GetClip(VoiceLine line)
    {
        switch (line)
        {
            case VoiceLine.Inicio: return inicio;
            case VoiceLine.Meteoritos: return meteoritos;
            case VoiceLine.PuertaCerrada: return puertaCerrada;
            case VoiceLine.PrimerGenerador: return primerGenerador;
            case VoiceLine.PuertaAbierta: return puertaAbierta;
            case VoiceLine.SesentaSegundos: return sesentaSegundos;
            default: return null;
        }
    }
}
