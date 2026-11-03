using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Zona donde el jugador puede interactuar con la tecla E.
// Va en cada generador, junto con un Collider marcado como "Is Trigger".
//
// Este script NO toca la UI: solo avisa a GameEvents que el jugador
// entro o salio de la zona, y la UI decide como mostrarse.
public class InteractZone : MonoBehaviour
{
    [Header("Configuracion")]
    [Tooltip("Tag del objeto del jugador")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("Si esta tildado, solo se puede interactuar una vez (ej: activar un generador)")]
    [SerializeField] private bool singleUse = true;

    [Header("Que pasa al apretar E")]
    [Tooltip("Aca se conecta desde el Inspector lo que tiene que pasar al interactuar")]
    public UnityEvent onInteract;

    // Si el jugador esta adentro de la zona en este momento
    private bool playerInside;

    // Si ya se uso (para los de un solo uso)
    private bool used;

    private void OnTriggerEnter(Collider other)
    {
        if (used || !IsPlayer(other)) return;

        playerInside = true;
        GameEvents.RaiseInteractPromptShow();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;

        playerInside = false;
        GameEvents.RaiseInteractPromptHide();
    }

    private void Update()
    {
        if (!playerInside || used) return;

        if (EPressed())
        {
            if (singleUse) used = true;

            GameEvents.RaiseInteractPromptHide();
            onInteract.Invoke();
        }
    }

    // El collider que entra puede ser una parte del jugador,
    // por eso tambien revisamos el objeto raiz.
    private bool IsPlayer(Collider other)
    {
        return other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag);
    }

    // Funciona tanto con el Input System nuevo como con el viejo
    private bool EPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.E);
#endif
    }
}