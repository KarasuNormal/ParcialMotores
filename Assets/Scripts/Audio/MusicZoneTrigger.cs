using UnityEngine;

// Zona invisible del mapa: cuando el jugador entra, la musica avanza al estado elegido.
[RequireComponent(typeof(Collider))]
public class MusicZoneTrigger : MonoBehaviour
{
    [SerializeField] private MusicStateController.MusicState estado = MusicStateController.MusicState.Tension;
    [SerializeField] private string playerTag = "Player";

    private void Reset()
    {
        // Al agregar el script, el collider se marca como trigger automaticamente
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (MusicStateController.Instance != null)
        {
            MusicStateController.Instance.GoToState(estado);
        }
    }
}
