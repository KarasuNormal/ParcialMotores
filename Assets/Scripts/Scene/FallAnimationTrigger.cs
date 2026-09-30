using System.Collections;
using CharacterScripts;
using UnityEngine;

public class FallAnimationTrigger : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController fallController;
    // El controller con la secuencia de caida (Tambaleo -> CaidaLibre)

    [SerializeField] private float fallSpeed = 10f;
    // Que tan rapido baja el personaje hasta llegar al Void

    [SerializeField] private float maxFallTime = 5f;
    // Tiempo maximo que lo seguimos bajando, por seguridad

    private bool hasTriggered = false;
    // Para que la animacion no arranque dos veces

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player"))
        {
            return;
        }

        hasTriggered = true;

        CharacterControllerCs controller = other.GetComponent<CharacterControllerCs>();
        if (controller != null)
        {
            controller.enabled = false;
            // Apagamos el control: a partir de aca ya no hay salvacion
        }

        Animator animator = other.GetComponent<Animator>();
        if (animator != null && fallController != null)
        {
            animator.runtimeAnimatorController = fallController;
            animator.applyRootMotion = false;
            animator.Play("Tambaleo");
            // Cambiamos al controller de caida y arrancamos directo en el tambaleo
        }

        StartCoroutine(KeepFalling(other.GetComponent<CharacterController>()));
    }

    private IEnumerator KeepFalling(CharacterController characterController)
    {
        float timer = 0f;

        while (characterController != null && timer < maxFallTime)
        {
            characterController.Move(Vector3.down * fallSpeed * Time.deltaTime);
            // Lo seguimos bajando hasta que toque el Void, que muestra la pantalla de perdiste
            timer += Time.deltaTime;
            yield return null;
        }
    }
}