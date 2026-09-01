using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

public class TiltingPlatform : DynamicPlatform
{
    [Header("Configuración de inclinación")]
    [SerializeField] private float tiltAngle = 35f;
    [SerializeField] private float tiltSpeed = 40f;

    private Rigidbody rb;

    private Quaternion initialRotation;
    private Quaternion targetRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;

        initialRotation = transform.rotation;

        targetRotation =
            initialRotation *
            Quaternion.Euler(0f, 0f, tiltAngle);
    }

    public override void Activate()
    {
        StartCoroutine(Tilt());
    }

    private IEnumerator Tilt()
    {
        while (Quaternion.Angle(rb.rotation, targetRotation) > 0.5f)
        {
            Quaternion newRotation =
                Quaternion.RotateTowards(
                    rb.rotation,
                    targetRotation,
                    tiltSpeed * Time.fixedDeltaTime
                );

            rb.MoveRotation(newRotation);

            yield return new WaitForFixedUpdate();
        }

        rb.MoveRotation(targetRotation);
    }
}
