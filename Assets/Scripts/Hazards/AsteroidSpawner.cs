using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject warningMarkerPrefab;

    [Header("Configuration")]
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float spawnHeight = 25f;
    [SerializeField] private float diagonalOffset = 20f;
    [SerializeField] private float fallSpeed = 25f;

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
        Vector3 targetPosition = player.position;

        GameObject marker = Instantiate(warningMarkerPrefab, targetPosition + (Vector3.up * 0.1f), Quaternion.identity);

        Vector3 spawnPosition = targetPosition + new Vector3(diagonalOffset, spawnHeight, diagonalOffset);

        GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

        AsteroidBehaviour behaviour = asteroid.GetComponent<AsteroidBehaviour>();
        if (behaviour != null)
        {
            behaviour.Initialize(targetPosition, fallSpeed, marker);
        }
    }
}