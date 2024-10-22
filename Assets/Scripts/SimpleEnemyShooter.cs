using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyShooter : MonoBehaviour
{
    public GameObject projectilePrefab;    // The projectile to shoot
    public float shootInterval = 2f;       // Time between shots
    public float projectileSpeed = 5f;     // How fast projectiles move
    
    private float nextShootTime;
    private Transform player;
    
    void Start()
    {
        // Find the player by tag
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    
    void Update()
    {
        if (player == null) return;
        
        // Check if it's time to shoot
        if (Time.time >= nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + shootInterval;
        }
    }
    
    void Shoot()
    {
        // Calculate direction to player
        Vector2 direction = (player.position - transform.position).normalized;
        
        // Create projectile
        GameObject projectile = Instantiate(projectilePrefab, (Vector2)transform.position + direction * 0.5f, Quaternion.identity);
        
        // Set projectile velocity
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }
    }
}