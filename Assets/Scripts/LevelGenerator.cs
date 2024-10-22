using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] platformPrefabs;
    public GameObject[] obstaclePrefabs;
    public float scrollSpeed = 5f;
    public float platformSpacing = 10f;
    public float minY = -5f;
    public float maxY = 5f;
    
    private float nextPlatformX;
    
    void Start()
    {
        nextPlatformX = transform.position.x + platformSpacing;
        GenerateInitialPlatforms();
    }
    
    void Update()
    {
        // Move everything left
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;
        
        // Generate new platforms as needed
        if (transform.position.x < nextPlatformX)
        {
            Debug.Log("Current position x: " + transform.position.x);
            
            GeneratePlatform();
            nextPlatformX += platformSpacing;
        }
        
        // Cleanup old platforms
        foreach (Transform child in transform)
        {
            if (child.position.x < transform.position.x - 30f)
            {
                Destroy(child.gameObject);
            }
        }
    }
    
    void GenerateInitialPlatforms()
    {
        for (int i = 0; i < 10; i++)
        {
            GeneratePlatform();
            nextPlatformX += platformSpacing;
        }
    }
    
    void GeneratePlatform()
    {
        // Random platform position
        float randomY = Random.Range(minY, maxY);
        Vector3 position = new Vector3(nextPlatformX, randomY, 0);
        
        // Spawn platform
        GameObject platform = Instantiate(
            platformPrefabs[Random.Range(0, platformPrefabs.Length)],
            position,
            Quaternion.identity,
            transform
        );
        
        // Maybe spawn obstacle
        if (Random.value > 0.7f)
        {
            Vector3 obstaclePos = position + new Vector3(Random.Range(-2f, 2f), 2f, 0);
            Instantiate(
                obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)],
                obstaclePos,
                Quaternion.identity,
                transform
            );
        }
    }
}