using CharacterScripts;
using UnityEngine;

public class AsteroidBehaviour : MonoBehaviour
{
    private GameObject warningMarker;

    // Audio del asteroide (bloque D - Rebeca)
    // El prefab tiene 3 AudioSource: los dos primeros son el silbido de la caida,
    // el tercero guarda el sonido de la explosion y su configuracion (grupo del mixer, distancias)
    private AudioSource flightAudioSource1;
    private AudioSource flightAudioSource2;
    private AudioSource explosionSource;

    public void Initialize(Vector3 targetPosition, float speed, GameObject marker)
    {
        warningMarker = marker;

        AudioSource[] audioSources = GetComponents<AudioSource>();

        if (audioSources.Length > 0) flightAudioSource1 = audioSources[0];
        if (audioSources.Length > 1) flightAudioSource2 = audioSources[1];
        if (audioSources.Length > 2) explosionSource = audioSources[2];

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;

            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (warningMarker != null)
        {
            Destroy(warningMarker);
        }

        // El silbido se corta al tocar algo
        if (flightAudioSource1 != null) flightAudioSource1.Stop();
        if (flightAudioSource2 != null) flightAudioSource2.Stop();

        PlayExplosion();

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("DEFEAT! Asteroid killed you.");


            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            CharacterControllerCs controller = collision.gameObject.GetComponent<CharacterControllerCs>();
            if (controller != null)
            {
                controller.enabled = false;
            }

            GameEvents.RaiseDefeat();
            // Avisamos que se perdio. La UI y el timer reaccionan solos
        }

        Destroy(gameObject);
    }

    private void PlayExplosion()
    {
        if (explosionSource == null || explosionSource.clip == null) return;

        // Creamos un objeto temporal en el punto del impacto para que la explosion
        // no se corte cuando se destruye el asteroide.
        // Copiamos la configuracion del AudioSource original para que salga
        // por el mismo grupo del mixer (SFX) y con las mismas distancias 3D.
        GameObject temp = new GameObject("AsteroidExplosion");
        temp.transform.position = transform.position;

        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = explosionSource.clip;
        source.outputAudioMixerGroup = explosionSource.outputAudioMixerGroup;
        source.volume = explosionSource.volume;
        source.spatialBlend = explosionSource.spatialBlend;
        source.rolloffMode = explosionSource.rolloffMode;
        source.minDistance = explosionSource.minDistance;
        source.maxDistance = explosionSource.maxDistance;
        source.Play();

        Destroy(temp, explosionSource.clip.length);
    }
}