using UnityEngine;

// Zona invisible del mapa: cuando el jugador entra, suena la linea de voz elegida (bloque E).
[RequireComponent(typeof(Collider))]
public class VoiceZoneTrigger : MonoBehaviour
{
    [SerializeField] private VoiceManager.VoiceLine linea = VoiceManager.VoiceLine.Meteoritos;
    [SerializeField] private string playerTag = "Player";

    private void Reset()
    {
        // Al agregar el script, el collider se marca como trigger automaticamente
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (VoiceManager.Instance != null)
        {
            VoiceManager.Instance.Play(linea);
        }
    }
}
