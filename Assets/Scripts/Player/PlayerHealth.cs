using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invulnerabilityTime = 0.5f;

    [Header("Audio - Bloque C")]
    [SerializeField] private AudioSource sfxDamage;

    private int currentHealth;
    private float nextDamageTime;
    private bool isDead;

    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        nextDamageTime = 0f;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || Time.time < nextDamageTime)
            return;

        currentHealth -= damage;
        nextDamageTime = Time.time + invulnerabilityTime;

        if (sfxDamage != null) sfxDamage.Play(); // Bloque C

        Debug.Log("Player health: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Player died. Sending defeat event.");

        GameEvents.RaiseDefeat();
    }
}