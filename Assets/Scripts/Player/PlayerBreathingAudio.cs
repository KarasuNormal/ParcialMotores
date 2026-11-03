
using UnityEngine;

public class PlayerBreathingAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource breathingSource;

    [Header("Movement Thresholds")]
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 6f;

    [Header("Breathing Volume")]
    [SerializeField] private float minVolume = 0.12f;
    [SerializeField] private float maxVolume = 0.5f;

    [Header("Breathing Pitch")]
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.25f;

    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 3f;

    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;

        breathingSource.loop = true;
        breathingSource.Play();
    }

    private void LateUpdate()
    {
        Vector3 movement = transform.position - lastPosition;
        movement.y = 0f;

        float speed = movement.magnitude / Time.deltaTime;

        lastPosition = transform.position;

        float targetVolume = 0f;
        float targetPitch = minPitch;

        if (speed > 0.15f)
        {
            float intensity = Mathf.InverseLerp(
                walkSpeed, runSpeed, speed
            );

            targetVolume = Mathf.Lerp(minVolume, maxVolume, intensity);
            targetPitch = Mathf.Lerp(minPitch, maxPitch, intensity);
        }

        float blend = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);

        breathingSource.volume = Mathf.Lerp(
            breathingSource.volume, targetVolume, blend
        );

        breathingSource.pitch = Mathf.Lerp(
            breathingSource.pitch, targetPitch, blend
        );
    }
}
