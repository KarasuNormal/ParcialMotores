using JetBrains.Annotations;
using UnityEngine;
using System.Collections;

public abstract class DynamicPlatform : MonoBehaviour
{
    [Header("General Configuration")]
    [SerializeField] protected float activationDelay = 0.5f;

    protected bool activated = false;

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (activated)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            activated = true;
            StartCoroutine(ActivateAfterDelay());
        }
    }

    private IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);

        Activate();

    }

    public abstract void Activate();

}
