using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomUFOs : MonoBehaviour
{
    public GameObject[] spaceshipPrefabs;

    public Transform[] spawnPoints;

    public AudioClip[] spawnSounds; // Optional: sounds to play when spawning

    public float speed = 5f;
    public float lifeTime = 6f;
    public float spawnInterval = 60f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnSpaceship();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    [ContextMenu("Spawn Spaceship Now")]
    void SpawnSpaceship()
    {
        if (spaceshipPrefabs.Length == 0 || spawnPoints.Length == 0)
            return;

        Debug.Log("yo");
        GameObject prefab = spaceshipPrefabs[Random.Range(0, spaceshipPrefabs.Length)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject ship = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        SoundFXManager.Instance.PlaySound(spawnSounds[Random.Range(0, spawnSounds.Length)], spawnPoint, 1.0f, 100, 1.0f, 1.0f);
        Vector3 randomDir = Random.onUnitSphere;
        randomDir.y = 0;

        ship.AddComponent<SpaceshipMover>().Init(randomDir.normalized, speed, lifeTime);
    }

 
}