using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject characterPrefab; // Reference to the character prefab in your Assets folder

    private float spawnTimer = 0f; // Timer to track when to spawn the character
    private float spawnInterval = 10f; // Time interval between character spawns

    private void Update()
    {
        // Increment the spawn timer based on Time.deltaTime
        spawnTimer += Time.deltaTime;

        // Check if the spawn timer exceeds the spawn interval
        if (spawnTimer >= spawnInterval)
        {
            SpawnCharacter();
            spawnTimer = 0f; // Reset the spawn timer
        }
    }

    private void SpawnCharacter()
    {
        // Instantiate the character prefab at the current position of the CharacterSpawner object
        Instantiate(characterPrefab, transform.position, Quaternion.identity);
    }
}