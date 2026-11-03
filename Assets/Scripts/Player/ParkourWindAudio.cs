
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ParkourWindAudio : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource windAudio;

    [Header("Speed Settings")]
    [SerializeField] private float minSpeed = 1.5f;
    [SerializeField] private float maxSpeed = 6f;

    [Header("Volume Settings")]
    [SerializeField] private float maxVolume = 0.4f;

    [Header("Pitch Settings")]
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.3f;

    [Header("Transition Settings")]
    [SerializeField] private float smoothSpeed = 4f;

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void LateUpdate()
    {
        Vector3 horizontalVelocity = characterController.velocity;
        horizontalVelocity.y = 0f;

        float speed = horizontalVelocity.magnitude;
        float intensity = Mathf.InverseLerp(minSpeed, maxSpeed, speed);

        UpdateAudio(intensity);
    }

    private void UpdateAudio(float intensity)
    {
        float targetVolume = intensity * maxVolume;
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, intensity); 
        windAudio.volume = Mathf.Lerp(windAudio.volume, targetVolume, 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime));

        windAudio.pitch = Mathf.Lerp(windAudio.pitch, targetPitch, 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime));
    }
}
