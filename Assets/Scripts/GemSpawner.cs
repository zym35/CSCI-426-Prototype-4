using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // The prefab of the object you want to spawn
    public float spawnRadius;   // The radius of the spawning area
    public int numberOfGems; // The number of objects to spawn

    void Start()
    {
        
    }

    void Update()
    {
        if (GetNumberOfLiveGems() < numberOfGems) {
            StartCoroutine(SpawnEverySecond());
        }
    }

    int GetNumberOfLiveGems() {
        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");
        int count = 0;

        foreach (GameObject gem in gems) {
            // Calculate the distance between this object and the object with the tag
            float distance = Vector3.Distance(transform.position, gem.transform.position);

            // Check if the object is within the specified radius
            if (distance <= spawnRadius)
            {
                count++;
            }
        }

        return count;
    }

    void SpawnGem() {
        // Generate a random position within the cylinder's radius
        Vector3 randomPosition = transform.position + Random.insideUnitSphere * spawnRadius;

        // Ensure the object spawns at the cylinder's height (adjust as needed)
        randomPosition.y = 2.5f;

        // Instantiate the object at the random position
        Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
    }

    IEnumerator SpawnEverySecond() {
        SpawnGem();
        yield return new WaitForSeconds(1f);
    }
}
