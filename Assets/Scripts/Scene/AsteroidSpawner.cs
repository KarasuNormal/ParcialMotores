using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float spawnHeight = 15f;
    [SerializeField] private float spawnRangeX = 5f;
    [SerializeField] private float spawnRangeZ = 5f;
    [SerializeField] private float forwardDistance = 15f;
    [SerializeField] private float fallSpeedBoost = 15f;

    [Header("Angled Fall")]
    [SerializeField] private float horizontalForce = 5f;

    private float timer;
    private Transform player;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.Log("Player not found. AsteroidSpawner will not spawn asteroids.");
        }
    }

    private void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnAsteroid();
        }
    }

    private void SpawnAsteroid()
    {
        Vector3 forwardPoint = player.position + player.forward * forwardDistance;

        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        float randomZ = Random.Range(-spawnRangeZ, spawnRangeZ);

        Vector3 spawnPosition = new Vector3(
            forwardPoint.x + randomX,
            player.position.y + spawnHeight,
            forwardPoint.z + randomZ
        );

        GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

        Rigidbody rb = asteroid.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            ).normalized;

            rb.AddForce(randomDirection * horizontalForce, ForceMode.Impulse);
            rb.AddForce(Vector3.down * fallSpeedBoost, ForceMode.Impulse);
        }

        Debug.Log("Asteroid spawned at " + spawnPosition);
    }
}